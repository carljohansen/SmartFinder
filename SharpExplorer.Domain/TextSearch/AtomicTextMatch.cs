namespace SharpExplorer.Domain.TextSearch;

public class AtomicTextMatch(string TextPattern, TextMatchingOptions Options)
{
    public int FindInLine(string line)
    {
        var comparisonType = Options.HasFlag(TextMatchingOptions.CaseSensitive) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        if (Options.HasFlag(TextMatchingOptions.WholeWord))
        {
            return line.IndexOfWholeWord(TextPattern, comparisonType);
        }
        else
        {
            return line.IndexOf(TextPattern, comparisonType);
        }
    }
}
