namespace RangeFinder.Domain.TextSearch;

public class FileMatcher
{
    private readonly string? baseDirectory;

    public FileMatcher(string? BaseDirectory, IEnumerable<AtomicTextMatch> atomicTextMatches)
    {
        baseDirectory = BaseDirectory;
        Matchers = atomicTextMatches.Select(atm => new LineTextMatcher(atm)).ToArray();
    }

    public bool HasTextChecks => Matchers.Length > 0;

    public int NumTextChecks => Matchers.Length;

    public readonly LineTextMatcher[] Matchers;

    private string GetFullPath(string filePath)
    {
        if (baseDirectory is null)
        {
            return filePath; // This better be absolute!
        }
        return Path.Combine(baseDirectory, filePath);
    }

    public IReadOnlyCollection<IFileLineDisplay> GetMatches(string filePath, int max = 200)
    {
        var numMatchingLinesFound = 0;

        var matches = new List<IFileLineDisplay>();
        var fullPath = GetFullPath(filePath);
        int lineNumber = 0;

        foreach (var line in File.ReadLines(fullPath))
        {
            lineNumber++;
            var lineHasMatch = false;
            foreach (var matcher in Matchers)
            {
                var match = matcher.GetMatch(line, lineNumber);
                if (match != null)
                {
                    matches.Add(match);
                    lineHasMatch = true;
                }
            }

            if (!lineHasMatch)
            {
                matches.Add(new FileLinePlainPreview(line, lineNumber));
            }
            else
            {
                numMatchingLinesFound++;
            }
        }
        return matches;
    }
}