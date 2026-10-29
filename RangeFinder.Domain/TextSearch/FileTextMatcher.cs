namespace RangeFinder.Domain.TextSearch;

public class FileTextMatcher(string? BaseDirectory, AtomicTextMatch TextMatch)
{
    public bool IsExclusion => TextMatch.IsExclusion;

    private string GetFullPath(string filePath)
    {
        if (BaseDirectory is null)
        {
            return filePath; // This better be absolute!
        }
        return Path.Combine(BaseDirectory, filePath);
    }

    public bool HasMatchOnLine(string line)
    {
        return TextMatch.FindInLine(line) >= 0;
    }

    public IReadOnlyCollection<IFileLineDisplay> GetMatches(string filePath, int max = 200)
    {
        var numFound = 0;
        if (TextMatch.IsExclusion)
        {
            return [];
        }

        var matches = new List<IFileLineDisplay>();
        var fullPath = GetFullPath(filePath);
        int lineNumber = 0;
        foreach (var line in File.ReadLines(fullPath))
        {
            lineNumber++;
            int startPos = TextMatch.FindInLine(line);
            if (startPos >= 0)
            {
                matches.Add(new FileLineTextMatch(line, lineNumber, startPos, TextMatch.Length, TextMatch));
                if (numFound++ >= max)
                {
                    return matches;
                }
            }
            else
            {
                matches.Add(new FileLinePlainPreview(line, lineNumber));
            }
        }
        return matches;
    }
}