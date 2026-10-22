namespace SharpExplorer.Domain.History;

public class UserTabSettings
{
    public required int Id { get; set; }

    public string? ResultSortOrder { get; set; }

    public int? SplitterDistance { get; set; }

    public UserTabSettings Clone() => new()
    {
        Id = this.Id,
        ResultSortOrder = this.ResultSortOrder,
        SplitterDistance = this.SplitterDistance
    };

    public static UserTabSettings CreateDefault() => new()
    {
        Id = 0,
        ResultSortOrder = null,
        SplitterDistance = null
    };
}

public record UserTabState(UserSearchDto SearchDto, UserTabSettings TabSettings)
{
    public int Id => SearchDto.Id;

    public static UserTabState CreateDefault() => new(UserSearchDto.CreateDefault(), UserTabSettings.CreateDefault());

    public static UserTabState Create(UserSearchDto searchDto, UserTabSettings tabSettings)
    {
        var result = new UserTabState(searchDto, tabSettings);
        if (tabSettings.Id != searchDto.Id)
        {
            tabSettings.Id = searchDto.Id;
        }

        return result;
    }
}