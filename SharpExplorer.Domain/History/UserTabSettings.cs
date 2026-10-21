namespace SharpExplorer.Domain.History;

public class UserTabSettings
{
    public string? ResultSortOrder { get; set; }

    public readonly static UserTabSettings Default = new() { ResultSortOrder = null };
}

public record UserTabState(UserSearchDto SearchDto, UserTabSettings TabSettings)
{
    public int Id => SearchDto.Id;

    public readonly static UserTabState Default = new(UserSearchDto.Default, new UserTabSettings());
}