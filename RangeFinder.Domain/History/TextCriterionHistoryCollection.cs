namespace RangeFinder.Domain.History;

public class TextCriterionHistoryCollection
{
    public readonly static TextCriterionHistoryCollection Instance = new();

    public readonly TextCriterionHistory BaseDirHistory = new DirectoryTextCriterionHistory(25, UserSearchRepository.BaseDirHistoryFileName);
    public readonly TextCriterionHistory ExclusionsHistory = new(15, UserSearchRepository.ExclusionsHistoryFileName);
    public readonly TextCriterionHistory InclusionsHistory = new(15, UserSearchRepository.InclusionsHistoryFileName);
    public readonly TextCriterionHistory SearchTextHistory = new(25, UserSearchRepository.SearchTextHistoryFileName);

    private TextCriterionHistoryCollection()
    {
    }

    public void EnsureRefreshed()
    {
        BaseDirHistory.Initialize();
        ExclusionsHistory.Initialize();
        InclusionsHistory.Initialize();
        SearchTextHistory.Initialize();
    }

    public void ClearAll()
    {
        BaseDirHistory.Clear();
        ExclusionsHistory.Clear();
        InclusionsHistory.Clear();
        SearchTextHistory.Clear();
    }
}
