namespace SharpExplorer.Domain.TextSearch;

public class AtomicTextMatch
{
    public readonly int Length;

    public readonly int Index;

    public readonly bool IsExclusion;

    public readonly bool HasLeadingWhitespace;
    private readonly string textPattern;
    private readonly TextMatchingOptions options;

    public AtomicTextMatch(string textPattern, TextMatchingOptions options, bool isExclusion, int index)
    {
        this.textPattern = textPattern;
        this.options = options;
        Length = textPattern.Length;
        Index = index;
        IsExclusion = isExclusion;
        HasLeadingWhitespace = textPattern.Length > 1 && textPattern[0] == ' ';
    }

    public int FindInLine(string line)
    {
        var comparisonType = options.HasFlag(TextMatchingOptions.CaseSensitive) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        if (options.HasFlag(TextMatchingOptions.WholeWord))
        {
            return line.IndexOfWholeWord(textPattern, comparisonType);
        }
        else
        {
            return line.IndexOf(textPattern, comparisonType);
        }
    }
}
