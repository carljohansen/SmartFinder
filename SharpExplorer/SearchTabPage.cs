using SharpExplorer;
using SharpExplorer.Domain;
using SharpExplorer.Domain.History;

public class SearchTabPage : TabPage
{
    private readonly SearchForm _parentForm;
    private readonly SearchTab _searchTab;

    private readonly string _originalTabTitle;

    private readonly Dictionary<FileSearchStatus, Color> _statusColors = new()
    {
        { FileSearchStatus.Running, Color.LightBlue },
        { FileSearchStatus.Completed, Color.LightGreen },
        { FileSearchStatus.Failed, Color.LightPink },
        { FileSearchStatus.Cancelled, Color.LightGoldenrodYellow },
        { FileSearchStatus.NotRunning, default },
    };

    public Color HeaderColor
    {
        get => field; private set
        {
            if (field != value)
            {
                field = value;
                _parentForm.RedrawTabControl();
            }
        }
    } = default;

    public SearchTabPage(SearchForm parentForm, SearchTab content, string initialTitle)
    {
        _parentForm = parentForm;
        _searchTab = content;
        Controls.Add(content); // first SearchTab_Resize call happens here
        Text = initialTitle;
        _originalTabTitle = initialTitle;

        _searchTab.FilesFoundUpdate += HandleFilesFoundUpdate;
        _searchTab.Width = this.Width;
        _searchTab.Height = this.Height;
    }

    public UserTabSettings GetCurrentUserSettings() => _searchTab.GetCurrentUserSettings();

    private int? _lastFileCountShown;

    private void HandleFilesFoundUpdate(object? sender, FilesFoundUpdateEventArgs e)
    {
        HeaderColor = _statusColors[e.SearchStatus];

        if (e.SearchStatus == FileSearchStatus.Running)
        {
            if (e.FilesFound <= 0)
            {
                _lastFileCountShown = -2;
                SetParentTabTitle("Running...");
                return;
            }
        }
        else
        {
            if (e.FilesFound <= 0)
            {
                _lastFileCountShown = -2;
                SetParentTabTitle("No match");
                HeaderColor = Color.Honeydew;
                return;
            }
        }

        if (e.FilesFound == _lastFileCountShown)
        {
            return;
        }

        _lastFileCountShown = e.FilesFound;

        var newTabTitle = e.FilesFound switch
        {
            1 => "1 file",
            < 1000 => $"{e.FilesFound} files",
            _ => "1000+"
        };

        SetParentTabTitle(newTabTitle);
    }

    public int Index => _searchTab.Index;

    public UserTabState GetCurrentSearchSettings() => _searchTab.GetCurrentTabState();

    public void ClearHistory() => _searchTab.ClearHistory();

    public void SetProgress(int progressPercentage) => SetProgress(progressPercentage);

    public void SetParentTabTitle(string title)
    {
        if (this.Text != title)
        {
            this.Text = title;
        }
    }

    public void CancelSearch()
    {
        _searchTab.CancelSearch();
    }

    public void InitialFocus() => _searchTab.InitialFocus();

    internal void ApplyUserPreferences() => _searchTab.ApplyUserPreferences();
}
