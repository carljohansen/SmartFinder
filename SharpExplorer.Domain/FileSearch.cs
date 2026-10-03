using SharpExplorer.Domain.TextSearch;

namespace SharpExplorer.Domain;

public class FileSearch
{
    public List<AtomicTextMatch> TextSearches { get; } = [];

    public IAsyncEnumerable<FileDisplay> SearchFilesAsync(string directoryPath, string pattern)
    {
        if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
        {
            throw new ArgumentException("Invalid directory path.", nameof(directoryPath));
        }
        if (string.IsNullOrWhiteSpace(pattern))
        {
            throw new ArgumentException("Invalid file pattern.", nameof(pattern));
        }
        return SearchFilesInternalAsync(directoryPath, pattern, [.. TextSearches.Select(ts => new FileTextMatcher(directoryPath, ts))]);
    }

    private async IAsyncEnumerable<FileDisplay> SearchFilesInternalAsync(string baseDiretory, string pattern, FileTextMatcher[] textChecks)
    {
        var rawFiles = FileSearcher.DoSearch(baseDiretory, pattern);
        if (!rawFiles.HasMatches)
        {
            yield break;
        }

        foreach (var match in rawFiles.Files)
        {
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
        }
    }

    public IReadOnlyCollection<FileLineTextMatch> GetMatches(string filePath)
    {
        if (TextSearches.Count == 0)
        {
            return [];
        }

        var firstChecker = new FileTextMatcher(null, TextSearches.First());
        return firstChecker.GetMatches(filePath);
    }
}