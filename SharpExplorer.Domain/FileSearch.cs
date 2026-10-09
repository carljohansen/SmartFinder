using SharpExplorer.Domain.TextSearch;
using System.Diagnostics;

namespace SharpExplorer.Domain;

public class FileSearch
{
    public List<AtomicTextMatch> TextSearches { get; } = [];

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

    private async IAsyncEnumerable<FileDisplay> SearchFilesInternalAsync(string baseDiretory,
                                                                        PathCollection includePatterns,
                                                                        PathCollection excludePatterns,
                                                                        FileTextMatcher[] textChecks,
                                                                        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            yield break;
        }

        var rawFiles = await Task.Run(() => FileSearcher.DoSearch(baseDiretory, includePatterns, excludePatterns));
        if (!rawFiles.HasMatches)
        {
            yield break;
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
                yield return new FileDisplay(match.Path, baseDiretory);
            }

            if (timer.ElapsedMilliseconds > 100)
            {
                await Task.Delay(1, cancellationToken);
                timer.Restart();
            }
        }
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

        return result;
    }
}