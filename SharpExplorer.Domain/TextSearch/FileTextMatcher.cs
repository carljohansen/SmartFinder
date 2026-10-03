namespace SharpExplorer.Domain.TextSearch;

public class FileTextMatcher(string? BaseDirectory, AtomicTextMatch TextMatch)
{
    private string GetFullPath(string filePath)
    {
        if (BaseDirectory is null)
        {
            return filePath; // This better be absolute!
        }
        return Path.Combine(BaseDirectory, filePath);
    }

    public bool HasMatch(string filePath)
    {
        foreach (var line in File.ReadLines(GetFullPath(filePath)))
        {
            if (TextMatch.FindInLine(line) >= 0)
            {
                return true;
            }
        }
        return false;
    }

    public IReadOnlyCollection<FileLineTextMatch> GetMatches(string filePath)
    {
        var matches = new List<FileLineTextMatch>();
        var fullPath = GetFullPath(filePath);
        int lineNumber = 0;
        foreach (var line in File.ReadLines(fullPath))
        {
            lineNumber++;
            int startPos = TextMatch.FindInLine(line);
            if (startPos >= 0)
            {
                matches.Add(new FileLineTextMatch(line, lineNumber, startPos, TextMatch.Length));
            }
        }
        return matches;
    }
}