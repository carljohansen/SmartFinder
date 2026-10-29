namespace RangeFinder.Domain.TextSearch;

public class LineTextMatcher(AtomicTextMatch TextMatch)
{
    public bool IsExclusion => TextMatch.IsExclusion;

    public bool HasMatchOnLine(string line)
    {
        return TextMatch.FindInLine(line) >= 0;
    }

    public IFileLineDisplay? GetMatch(string? line, int lineNumber)
    {
        if (line == null
            || TextMatch.IsExclusion)
        {
            return null;
        }

        int startPos = TextMatch.FindInLine(line);
        if (startPos >= 0)
        {
            return new FileLineTextMatch(line, lineNumber, startPos, TextMatch.Length, TextMatch);
        }
        else
        {
            return null;
        }
    }
}