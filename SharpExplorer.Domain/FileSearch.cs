using SharpExplorer.Domain.TextSearch;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SharpExplorer.Domain;

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
        if (search.HasTextSearch())
        {
            var matchFragments = search.ParseTextSearchPattern();
            _textSearches.AddRange(matchFragments);
        }
    }

    public void SetLocationDisplayLength(int displayLength)
    {
        _fileLocationRepository.SetDisplayLength(displayLength);
    }

    private string? _currentStatusReportingPath = null;
    private string? _currentStatusReportedDirectory = null;

    public void Reset()
    {
        _textSearches.Clear();
        _currentStatusReportedDirectory = null;
        _currentStatusReportingPath = null;
    }

    public async Task<string> SearchAndPopulateTableAsync(UserSearchDto searchDto, CancellationToken cancellationToken)
    {
        bool wasCancelled = await SearchAndPopulateTableAsyncInternal(searchDto, cancellationToken);

        var cancelledMsg = wasCancelled ? " (search cancelled)" : "";
        var numMatches = DataTable.Rows.Count;
        if (numMatches > 0)
        {
            return numMatches == 1 ? $"1 file found{cancelledMsg}" : $"{numMatches:#,##0} files found{cancelledMsg}";
        }
        else
        {
            return $"No matching files found{cancelledMsg}";
        }
    }

    private async Task<bool> SearchAndPopulateTableAsyncInternal(UserSearchDto searchDto, CancellationToken cancellationToken)
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

        return wasCancelled;
    }

    public IAsyncEnumerable<FileDisplay> SearchFilesAsync(string directoryPath,
                                                            PathCollection includePatterns,
                                                            PathCollection excludePatterns,
                                                            CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
        {
            throw new ArgumentException("Invalid directory path.", nameof(directoryPath));
        }

        return SearchFilesInternalAsync(directoryPath,
                                        includePatterns,
                                        excludePatterns,
                                        [.. TextSearches.Select(ts => new FileTextMatcher(directoryPath, ts))],
                                        cancellationToken);
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

        SearchProgressUpdate?.Invoke(this, new SearchProgressEventArgs { StatusMessage = "Building file list...", ProgressPercentage = 0 });

        var rawFiles = await Task.Run(() =>
        {
            if (includePatterns.Paths.Count == 0
                && textChecks.Length > 0)
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

        if (TextSearches.Count > 0)
        {
            SearchProgressUpdate?.Invoke(this, new SearchProgressEventArgs { StatusMessage = "Searching in files...", ProgressPercentage = 0 });
        }

        var timer = Stopwatch.StartNew();
        FileDisplay? matchedItem = null;
        var numFilesProcessed = 0;
        var numFiles = rawFiles.Files.Count();

        foreach (var match in rawFiles.Files)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            var passesMetadataChecks = true;
            if (SearchCriteria.HasMetadataCriteria)
            {
                matchedItem = new FileDisplay(match.Path, baseDirectory, _fileLocationRepository);
                matchedItem.InitializeMetadata();
                passesMetadataChecks = MeetsMetadataCriteria(matchedItem);
            }

            var passesTextCheck = true;
            if (textChecks.Length > 0)
            {
                var checkerState = new bool[textChecks.Length]; // false = still evaluating; true already found a match

                foreach (var line in File.ReadLines(Path.Combine(baseDirectory, match.Path)))
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
                yield return matchedItem ?? new FileDisplay(match.Path, baseDirectory, _fileLocationRepository);
            }

            numFilesProcessed++;

            if (timer.ElapsedMilliseconds > 100)
            {
                await Task.Delay(1, cancellationToken);
                timer.Restart();
                var percentComplete = (int)Math.Floor((numFilesProcessed * 100.0) / numFiles);
                SearchProgressUpdate?.Invoke(this, new SearchProgressEventArgs
                {
                    StatusMessage = $"Searching in files ({GetDirectoryForStatusReport(match.Path, baseDirectory)})...",
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