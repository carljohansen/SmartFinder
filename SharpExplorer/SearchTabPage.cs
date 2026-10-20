using SharpExplorer;
using SharpExplorer.Domain.History;

public class SearchTabPage : TabPage
{
    private readonly SearchForm _parentForm;
    private readonly SearchTab _searchTab;

    public SearchTabPage(SearchForm parentForm, SearchTab content)
    {
        _parentForm = parentForm;
        _searchTab = content;
        Controls.Add(content);

        _searchTab.Width = this.Width;
        _searchTab.Height = this.Height;
    }

    public int Index => _searchTab.Index;

    public UserSearchDto GetCurrentSearchSettings() => _searchTab.GetCurrentSettings();

    public void ClearHistory() => _searchTab.ClearHistory();

    public void SetProgress(int progressPercentage) => SetProgress(progressPercentage);

    public void SetParentTabTitle(string title) => this.Text = title;

    public void CancelSearch()
    {
        _searchTab.CancelSearch();
    }

    public void InitialFocus() => _searchTab.InitialFocus();
}
