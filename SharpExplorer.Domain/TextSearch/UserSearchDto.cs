namespace SharpExplorer.Domain.TextSearch;

public class UserSearchDto
{
    public required string RawIncludeFilesPattern { get; set; }

    public required string RawExcludeFilesPattern { get; set; }

    public string? TextSearchPattern { get; set; }

    public TextMatchingOptions TextMatchingOptions { get; set; } = TextMatchingOptions.None;

    public required string BaseDirectoriesPattern { get; set; }

    public bool HasTextSearch() => !string.IsNullOrWhiteSpace(TextSearchPattern);
}