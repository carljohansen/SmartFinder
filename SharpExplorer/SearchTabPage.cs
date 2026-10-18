using SharpExplorer;
using SharpExplorer.Domain;

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

    public UserSearchDto GetCurrentSearchSettings() => _searchTab.GetCurrentSettings();

    public void SetStatusText(string text) => _parentForm.SetStatusText(text);

    public void SetProgress(int progressPercentage) => _parentForm.SetProgress(progressPercentage);

    public void SetParentTabTitle(string title) => this.Text = title;

    public void CancelSearch()
    {
        _searchTab.CancelSearch();
        _parentForm.HideProgressBar();
        _parentForm.SetStatusText(null);
    }
}
