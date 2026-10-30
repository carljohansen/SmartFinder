using Microsoft.Extensions.FileSystemGlobbing;
using RangeFinder.Domain.History;
using RangeFinder.Domain.TextSearch;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RangeFinder.Domain;

public class FileSearch
{
    public IReadOnlyCollection<AtomicTextMatch> TextSearches => _textSearches;
    private readonly List<AtomicTextMatch> _textSearches = [];

    public readonly DataTable DataTable;

    private readonly FileLocationRepository _fileLocationRepository;

    private readonly IProgress<SearchProgressReportArgs>? _progressCallback = null;

    public UserSearchDto? SearchCriteria { get; private set; }

    public FileSearch(DataTable dataTable, int locationDisplayLength, IProgress<SearchProgressReportArgs>? progressCallback)
    {
        DataTable = dataTable;
        _fileLocationRepository = new(locationDisplayLength);
        _progressCallback = progressCallback;
    }

    public void SetSearch(UserSearchDto search)
    {
        SearchCriteria = search;
        _textSearches.Clear();

        _fileLocationRepository.Start(search.GetNormalizedBaseDirectory());

        if (search.HasTextSearch())
        {
            var matchFragments = search.ParseTextSearchPattern();
            _textSearches.AddRange(matchFragments);
        }
    }

    private const int LocationDataTableColumnIndex = 3;

    public void SetLocationDisplayLength(int displayLength)
    {
        _fileLocationRepository.SetDisplayLength(displayLength);

        foreach (var row in DataTable.Rows)
        {
            if (row is DataRow dataRow)
            {
                if (dataRow["FileDisplay"] is FileDisplay fileDisplay)
                {
                    dataRow[LocationDataTableColumnIndex] = new LocationDisplayComparable(fileDisplay.Location);
                }
            }
        }
    }

    private string? _currentStatusReportingPath = null;
    private string? _currentStatusReportedDirectory = null;

    public void Reset()
    {
        _textSearches.Clear();
        _currentStatusReportedDirectory = null;
        _currentStatusReportingPath = null;
    }

    public async Task<SearchOutcome> SearchAndPopulateTableAsync(UserSearchDto searchDto, CancellationToken cancellationToken)
    {
        var timer = Stopwatch.StartNew();

        var searchCompletionStatus = await OptimisticSearch(searchDto, cancellationToken);

        if (searchCompletionStatus == FileSearchStatus.UnauthorizedEx)
        {
            return new SearchOutcome("Encountered inaccessible folder", null, null, FileSearchStatus.UnauthorizedEx);
        }

        var wasCancelled = searchCompletionStatus == FileSearchStatus.Cancelled;

        string elapsedTimeMessage = $" in {SearchOutcome.GetElapsedTimeDisplay(timer.Elapsed)}";

        var finalState = wasCancelled ? FileSearchStatus.Cancelled : FileSearchStatus.Completed;
        var numMatches = DataTable.Rows.Count;

        string finalMessage = wasCancelled ? "Cancelled" : "Done";

        return new SearchOutcome(finalMessage, numMatches, timer.Elapsed, finalState);
    }

    private async Task<FileSearchStatus> OptimisticSearch(UserSearchDto searchDto,
                                                          CancellationToken cancellationToken)
    {
        var directoryPath = searchDto.GetNormalizedBaseDirectory();
        if (string.IsNullOrWhiteSpace(directoryPath)
            || !Path.IsPathFullyQualified(directoryPath)
            || !Directory.Exists(directoryPath))
        {
            throw new ArgumentException("Invalid directory path.", nameof(directoryPath));
        }

        List<string>? inaccessibleFolderExclusions = null;
        if (DirectoryEnumerator.Instance.HasCachedResult(directoryPath))
        {
            // We've got it; might as well use it.
            inaccessibleFolderExclusions = DirectoryEnumerator.Instance.GetInaccessibleSubdirectories(directoryPath, cancellationToken);
        }

        var firstSearchStatus = await SearchAndPopulateTableAsyncInternal(searchDto, inaccessibleFolderExclusions, cancellationToken);
        if (firstSearchStatus != FileSearchStatus.UnauthorizedEx)
        {
            return firstSearchStatus;
        }

        _progressCallback?.Report(new SearchProgressReportArgs { StatusMessage = "Verifying permissions", ProgressPercentage = -1 });

        inaccessibleFolderExclusions = await Task.Run(() => DirectoryEnumerator.Instance.GetInaccessibleSubdirectories(directoryPath, cancellationToken));

        if (cancellationToken.IsCancellationRequested)
        {
            return FileSearchStatus.Cancelled;
        }

        var secondSearchStatus = await SearchAndPopulateTableAsyncInternal(searchDto, inaccessibleFolderExclusions, cancellationToken);
        return secondSearchStatus;
    }

    private async Task<FileSearchStatus> SearchAndPopulateTableAsyncInternal(UserSearchDto searchDto,
                                                                            List<string>? inaccessibleFolderExclusions,
                                                                            CancellationToken cancellationToken)
    {
        var options = new FileSearchOptions { AutomaticTextSearchFileFiltering = true };

        var optionsForSearchPattern = (searchDto.HasTextSearch()
                                       && options.AutomaticTextSearchFileFiltering)
                                            ? PathCollectionOptions.None
                                            : PathCollectionOptions.AddGeneralWildcardIfBlank;

        var searchPatternCollection = PathCollection.Create(searchDto.RawIncludeFilesPattern, optionsForSearchPattern);
        var excludeSearchPatternCollection = PathCollection.Create(searchDto.GetExcludeFilesPattern(), PathCollectionOptions.None);
        if (inaccessibleFolderExclusions != null)
        {
            excludeSearchPatternCollection.AddRange(inaccessibleFolderExclusions);
        }

        Reset();
        DataTable.Clear();

        SetSearch(searchDto);

        var wasCancelled = false;

        if (searchDto.IncludeFolders
            && !searchDto.HasTextSearch())
        {
            _progressCallback?.Report(new SearchProgressReportArgs { StatusMessage = "Building directory list", ProgressPercentage = -1 });

            var unfilteredFolderList = await Task.Run(() => DirectorySearch.GetAllSubdirectories(searchDto.GetNormalizedBaseDirectory(), cancellationToken));
            if (cancellationToken.IsCancellationRequested)
            {
                return FileSearchStatus.Cancelled;
            }

            var filteredFolderList = FileSearcher.FilterPaths(unfilteredFolderList.DirectoryNames, searchPatternCollection, excludeSearchPatternCollection);
            foreach (var matchingDir in filteredFolderList.Files)
            {
                var folderDisplay = FileDisplay.CreateFolder(matchingDir.Path, searchDto.GetNormalizedBaseDirectory(), _fileLocationRepository);
                folderDisplay.AddToDataTable(DataTable);
            }
        }

        try
        {
            await foreach (var item in SearchFilesAsync(searchDto.GetNormalizedBaseDirectory(),
                                                                        searchPatternCollection,
                                                                        excludeSearchPatternCollection,
                                                                        cancellationToken))
            {
                item.InitializeMetadata();
                item.AddToDataTable(DataTable);
            }
        }
        catch (TaskCanceledException)
        {
            wasCancelled = true;
        }
        catch (UnauthorizedAccessException)
        {
            return FileSearchStatus.UnauthorizedEx;
        }

        wasCancelled |= cancellationToken.IsCancellationRequested;

        return wasCancelled ? FileSearchStatus.Cancelled : FileSearchStatus.Completed;
    }

    public async IAsyncEnumerable<FileDisplay> SearchFilesAsync(string directoryPath,
                                                            PathCollection includePatterns,
                                                            PathCollection excludePatterns,
                                                            [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
        {
            throw new ArgumentException("Invalid directory path.", nameof(directoryPath));
        }

        await foreach (var val in SearchFilesInternalAsync(directoryPath,
                                        includePatterns,
                                        excludePatterns,
                                        new FileMatcher(directoryPath, TextSearches),
                                        cancellationToken))
        {
            yield return val;
        }
    }

    private async IAsyncEnumerable<FileDisplay> SearchFilesInternalAsync(string baseDirectory,
                                                                        PathCollection includePatterns,
                                                                        PathCollection excludePatterns,
                                                                        FileMatcher textChecks,
                                                                        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            yield break;
        }

        if (SearchCriteria == null)
            yield break; // Should not happen.

        var hasTextChecks = textChecks.HasTextChecks;

        _progressCallback?.Report(new SearchProgressReportArgs { StatusMessage = "Building file list", ProgressPercentage = -1 });

        var rawFiles = await Task.Run(() =>
        {
            if (includePatterns.Paths.Count == 0
                && hasTextChecks)
            {
                return FileSearcher.DoNotBinarySearch(baseDirectory, excludePatterns);
            }
            else
            {
                return FileSearcher.DoSearch(baseDirectory, includePatterns, excludePatterns);
            }
        });
        if (!rawFiles.HasMatches)
        {
            yield break;
        }

        await foreach (var match in ProcessFiles(baseDirectory, rawFiles.Files, textChecks, cancellationToken))
        {
            yield return match;
        }
    }

    private async IAsyncEnumerable<FileDisplay> ProcessFiles(string baseDirectory,
                                                            IEnumerable<FilePatternMatch> files,
                                                            FileMatcher textChecks,
                                                            [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var hasTextChecks = textChecks.HasTextChecks;
        var actionMsg = hasTextChecks ? "Searching in files" : "Annotating files";

        if (hasTextChecks)
        {
            _progressCallback?.Report(new SearchProgressReportArgs { StatusMessage = $"{actionMsg}", ProgressPercentage = 0 });
        }

        var eventStreamTimer = Stopwatch.StartNew();

        FileDisplay? matchedItem = null;
        var numFilesProcessed = 0;
        var numFiles = files.Count();
        var numInclusiveChecks = textChecks.NumInclusiveChecks;
        var hasExclusiveChecks = textChecks.NumExclusiveChecks > 0;

        foreach (var match in files)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            var passesMetadataChecks = true;
            if (SearchCriteria.HasMetadataCriteria)
            {
                matchedItem = new FileDisplay(match.Path, _fileLocationRepository);
                matchedItem.InitializeMetadata();
                passesMetadataChecks = MeetsMetadataCriteria(matchedItem);
            }

            var passesTextCheck = true;
            var canShortCircuit = false;
            if (hasTextChecks)
            {
                var checkerState = new bool[textChecks.NumTextChecks]; // false = still evaluating; true already found a match
                var numInclusiveChecksSatisfied = 0;
                try
                {
                    foreach (var line in File.ReadLines(Path.Combine(_fileLocationRepository.BaseDirectory, match.Path)))
                    {
                        for (var i = 0; i < textChecks.NumTextChecks; i++)
                        {
                            if (checkerState[i] == false)
                            {
                                if (textChecks.Matchers[i].HasMatchOnLine(line))
                                {
                                    if (textChecks.Matchers[i].IsExclusion)
                                    {
                                        passesTextCheck = false;
                                        canShortCircuit = true;
                                        break;
                                    }
                                    checkerState[i] = true;
                                    numInclusiveChecksSatisfied++;
                                    if (numInclusiveChecksSatisfied == numInclusiveChecks
                                        && !hasExclusiveChecks)
                                    {
                                        canShortCircuit = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (canShortCircuit)
                        {
                            break;
                        }
                    }
                }
                catch (IOException)
                {
                    // If we can't read the file, we just skip it.
                    passesTextCheck = false;
                }

                for (var i = 0; i < textChecks.NumTextChecks; i++)
                {
                    if (textChecks.Matchers[i].IsExclusion)
                    {
                        checkerState[i] = true;
                    }
                }

                passesTextCheck &= checkerState.All(x => x == true); // If any of the text checks were not satisfied, we fail the file.
            }

            if (passesTextCheck
              && passesMetadataChecks)
            {
                yield return matchedItem ?? new FileDisplay(match.Path, _fileLocationRepository);
            }

            numFilesProcessed++;

            if (eventStreamTimer.ElapsedMilliseconds > 150)
            {
                await Task.Delay(1, cancellationToken);
                eventStreamTimer.Restart();
                var percentComplete = (int)Math.Floor((numFilesProcessed * 100.0) / numFiles);
                _progressCallback?.Report(new SearchProgressReportArgs
                {
                    StatusMessage = $"{actionMsg} ({GetDirectoryForStatusReport(match.Path, baseDirectory)})",
                    ProgressPercentage = percentComplete
                });
            }
        }
    }

    private bool MeetsMetadataCriteria(FileDisplay item)
    {
        if (SearchCriteria!.ModifiedBefore.HasValue
            && item.LastModified > SearchCriteria.ModifiedBefore)
        {
            return false;
        }
        if (SearchCriteria!.ModifiedAfter.HasValue
            && item.LastModified < SearchCriteria.ModifiedAfter)
        {
            return false;
        }
        //if (!SearchCriteria.ShowHiddenItems
        //    && item.Attributes.HasFlag(FileAttributes.Hidden))
        //{
        //    return false;
        //}
        return true;
    }

    private string GetDirectoryForStatusReport(string path, string baseDirectory)
    {
        if (path != _currentStatusReportingPath)
        {
            _currentStatusReportingPath = path;
            _currentStatusReportedDirectory = Path.GetDirectoryName(Path.GetFullPath(Path.Combine(baseDirectory, path)));
        }

        return _currentStatusReportedDirectory!;
    }

    public void GetMatches(string filePath, Action<IFileLineDisplay> matchAction)
    {
        if (TextSearches.Count == 0)
        {
            return;
        }

        var matcher = new FileMatcher(null, TextSearches);
        matcher.GetMatches(filePath, matchAction);
    }

    private int FileLineTextMatchSorter(IFileLineDisplay x, IFileLineDisplay y)
    {
        var lineNumCompare = x.LineNumber.CompareTo(y.LineNumber);
        if (lineNumCompare != 0)
            return lineNumCompare;
        return x.SecondarySortKey.CompareTo(y.SecondarySortKey);
    }
}