using Microsoft.Extensions.FileSystemGlobbing;
using RangeFinder.Domain.History;
using RangeFinder.Domain.TextSearch;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RangeFinder.Domain;

public record SearchOutcome(string StatusMessage, FileSearchStatus FinalState);

public class FileSearch
{
    public event EventHandler<SearchProgressEventArgs>? SearchProgressUpdate;

    public IReadOnlyCollection<AtomicTextMatch> TextSearches => _textSearches;
    private readonly List<AtomicTextMatch> _textSearches = [];

    public readonly DataTable DataTable;

    private readonly FileLocationRepository _fileLocationRepository;

    public UserSearchDto? SearchCriteria { get; private set; }

    public FileSearch(DataTable dataTable, int locationDisplayLength)
    {
        DataTable = dataTable;
        _fileLocationRepository = new(locationDisplayLength);
    }

    public void SetSearch(UserSearchDto search)
    {
        SearchCriteria = search;
        _textSearches.Clear();

        _fileLocationRepository.Start(search.BaseDirectoriesPattern);

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

        var searchCompletionStatus = await SearchAndPopulateTableAsyncInternal(searchDto, cancellationToken);

        if (searchCompletionStatus == FileSearchStatus.UnauthorizedEx)
        {
            return new SearchOutcome("Encountered inaccessible folder", FileSearchStatus.UnauthorizedEx);
        }

        var wasCancelled = searchCompletionStatus == FileSearchStatus.Cancelled;

        var cancelledMsg = wasCancelled ? " (search cancelled)" : "";

        string elapsedTimeMessage = $" in {GetElapsedTimeDisplay(timer.Elapsed)}";

        var finalState = wasCancelled ? FileSearchStatus.Cancelled : FileSearchStatus.Completed;
        var numMatches = DataTable.Rows.Count;

        string finalMessage = numMatches > 0
                                ? numMatches == 1 ? $"1 file found{cancelledMsg}" : $"{numMatches:#,##0} files found{elapsedTimeMessage}{cancelledMsg}"
                                : $"No matching files found{cancelledMsg}";

        return new SearchOutcome(finalMessage, finalState);
    }

    private object GetElapsedTimeDisplay(TimeSpan elapsed)
    {
        if (elapsed.TotalSeconds < 1)
        {
            return $"{elapsed.TotalMilliseconds:#,##0} ms";
        }
        else if (elapsed.TotalMinutes < 2)
        {
            return $"{elapsed.TotalSeconds:#,##0} seconds";
        }
        else if (elapsed.TotalHours < 1)
        {
            return $"{elapsed.TotalMinutes:#,##0} minutes";
        }
        else
        {
            return $"{elapsed.TotalHours:#,##0} hours";
        }
    }

    private async Task<FileSearchStatus> SearchAndPopulateTableAsyncInternal(UserSearchDto searchDto, CancellationToken cancellationToken)
    {
        var options = new FileSearchOptions { AutomaticTextSearchFileFiltering = true };

        var optionsForSearchPattern = (searchDto.HasTextSearch()
                                       && options.AutomaticTextSearchFileFiltering)
                                            ? PathCollectionOptions.None
                                            : PathCollectionOptions.AddGeneralWildcardIfBlank;

        var searchPatternCollection = PathCollection.Create(searchDto.RawIncludeFilesPattern, optionsForSearchPattern);
        var excludeSearchPatternCollection = PathCollection.Create(searchDto.GetExcludeFilesPattern(), PathCollectionOptions.None);

        Reset();
        DataTable.Clear();

        SetSearch(searchDto);

        var wasCancelled = false;
        try
        {
            await foreach (var item in SearchFilesAsync(searchDto.BaseDirectoriesPattern,
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

        // TODO: Don't put this here.  Put this method back the way it was,
        // (i.e. not async).
        SearchProgressUpdate?.Invoke(this, new SearchProgressEventArgs { StatusMessage = "Verifying permissions...", ProgressPercentage = 0 });

        var inaccessiblePaths = await Task.Run(() => DirectoryEnumerator.Instance.GetInaccessibleSubdirectories(directoryPath));
        excludePatterns.AddRange(inaccessiblePaths);

        await foreach (var val in SearchFilesInternalAsync(directoryPath,
                                        includePatterns,
                                        excludePatterns,
                                        [.. TextSearches.Select(ts => new FileTextMatcher(directoryPath, ts))],
                                        cancellationToken))
        {
            yield return val;
        }
    }

    private async IAsyncEnumerable<FileDisplay> SearchFilesInternalAsync(string baseDirectory,
                                                                        PathCollection includePatterns,
                                                                        PathCollection excludePatterns,
                                                                        FileTextMatcher[] textChecks,
                                                                        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            yield break;
        }

        if (SearchCriteria == null)
            yield break; // Should not happen.

        var hasTextChecks = textChecks.Length > 0;

        SearchProgressUpdate?.Invoke(this, new SearchProgressEventArgs { StatusMessage = "Building file list...", ProgressPercentage = 0 });

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
                                                            FileTextMatcher[] textChecks,
                                                            [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var hasTextChecks = textChecks.Length > 0;
        var actionMsg = hasTextChecks ? "Searching in files" : "Annotating files";

        if (hasTextChecks)
        {
            SearchProgressUpdate?.Invoke(this, new SearchProgressEventArgs { StatusMessage = $"{actionMsg}...", ProgressPercentage = 0 });
        }

        var eventStreamTimer = Stopwatch.StartNew();

        FileDisplay? matchedItem = null;
        var numFilesProcessed = 0;
        var numFiles = files.Count();

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
            if (hasTextChecks)
            {
                var checkerState = new bool[textChecks.Length]; // false = still evaluating; true already found a match

                try
                {
                    foreach (var line in File.ReadLines(Path.Combine(_fileLocationRepository.BaseDirectory, match.Path)))
                    {
                        for (var i = 0; i < textChecks.Length; i++)
                        {
                            if (checkerState[i] == false)
                            {
                                if (textChecks[i].HasMatchOnLine(line))
                                {
                                    if (textChecks[i].IsExclusion)
                                    {
                                        passesTextCheck = false;
                                        break;
                                    }
                                    checkerState[i] = true;
                                }
                            }
                        }

                        if (!passesTextCheck)
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

                for (var i = 0; i < textChecks.Length; i++)
                {
                    if (textChecks[i].IsExclusion)
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
                SearchProgressUpdate?.Invoke(this, new SearchProgressEventArgs
                {
                    StatusMessage = $"{actionMsg} ({GetDirectoryForStatusReport(match.Path, baseDirectory)})...",
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
        if (!SearchCriteria.ShowHiddenItems
            && item.Attributes.HasFlag(FileAttributes.Hidden))
        {
            return false;
        }
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

    public IReadOnlyCollection<FileLineTextMatch> GetMatches(string filePath)
    {
        if (TextSearches.Count == 0)
        {
            return [];
        }

        var result = new List<FileLineTextMatch>();
        foreach (var match in TextSearches)
        {
            var checker = new FileTextMatcher(null, match);
            result.AddRange(checker.GetMatches(filePath));
        }

        result.Sort(FileLineTextMatchSorter);
        return result;
    }

    private int FileLineTextMatchSorter(FileLineTextMatch x, FileLineTextMatch y)
    {
        return x.LineNumber.CompareTo(y.LineNumber);
    }
}