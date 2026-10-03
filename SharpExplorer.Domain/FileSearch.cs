namespace SharpExplorer.Domain;

public class FileSearch
{
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
        return SearchFilesInternalAsync(directoryPath, pattern);
    }

    private async IAsyncEnumerable<FileDisplay> SearchFilesInternalAsync(string baseDiretory, string pattern)
    {
        var rawFiles = FileSearcher.DoSearch(baseDiretory, pattern);
        if (!rawFiles.HasMatches)
        {
            yield break;
        }

        foreach (var match in rawFiles.Files)
        {
            yield return new FileDisplay(match.Path, baseDiretory);
        }
    }
}