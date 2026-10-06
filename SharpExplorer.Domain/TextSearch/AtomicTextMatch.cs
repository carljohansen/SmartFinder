namespace SharpExplorer.Domain.TextSearch;

public class AtomicTextMatch(string textPattern, TextMatchingOptions options, bool isExclusion)
{
    public readonly int Length = textPattern.Length;

    public readonly bool IsExclusion = isExclusion;

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
