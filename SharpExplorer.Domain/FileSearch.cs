using SharpExplorer.Domain.TextSearch;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SharpExplorer.Domain;

public class FileSearch
{
    public event EventHandler<SearchProgressEventArgs>? SearchProgressUpdate;

    public List<AtomicTextMatch> TextSearches { get; } = [];

    private string? _currentStatusReportingPath = null;
    private string? _currentStatusReportedDirectory = null;

    public void Reset()
    {
        TextSearches.Clear();
        _currentStatusReportedDirectory = null;
        _currentStatusReportingPath = null;
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

        SearchProgressUpdate?.Invoke(this, new SearchProgressEventArgs { StatusMessage = "Building file list..." });

        var rawFiles = await Task.Run(() => FileSearcher.DoSearch(baseDirectory, includePatterns, excludePatterns));
        if (!rawFiles.HasMatches)
        {
            yield break;
        }

        if (TextSearches.Count > 0)
        {
            SearchProgressUpdate?.Invoke(this, new SearchProgressEventArgs { StatusMessage = "Searching in files..." });
        }

        var timer = Stopwatch.StartNew();

        foreach (var match in rawFiles.Files)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }
            var passesTextCheck = true;
            if (TextSearches.Count > 0)
            {
                foreach (var textChecker in textChecks)
                {
                    if (!textChecker.HasMatch(match.Path))
                    {
                        passesTextCheck = false;
                        continue;
                    }
                }
            }

            if (passesTextCheck)
            {
                yield return new FileDisplay(match.Path, baseDirectory);
            }

            if (timer.ElapsedMilliseconds > 100)
            {
                await Task.Delay(1, cancellationToken);
                timer.Restart();
                SearchProgressUpdate?.Invoke(this, new SearchProgressEventArgs { StatusMessage = $"Searching in files ({GetDirectoryForStatusReport(match.Path, baseDirectory)})..." });
            }
        }
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