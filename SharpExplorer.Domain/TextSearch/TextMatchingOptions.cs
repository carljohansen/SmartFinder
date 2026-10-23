namespace RangeFinder.Domain.TextSearch;

[Flags]
public enum TextMatchingOptions
{
    None = 0,
    CaseSensitive = 1 << 0,
    WholeWord = 1 << 1,
}
