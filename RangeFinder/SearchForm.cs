using RangeFinder.Domain;
using RangeFinder.Domain.History;
using System.ComponentModel;
using System.Diagnostics;

namespace RangeFinder;

public partial class SearchForm : Form
{
    private int _nextTabId = 1;

    [DefaultValue(null)]
    public string? InitialOpenZipFileName
    {
        get => field;
        internal set
        {
            field = value;
            var verified = ZipExpander.VerifyZipFileByName(value);
            if (verified != null)
            {
                _verifiedInitialOpenZipFileName = verified;
            }
        }
    }

    private string? _verifiedInitialOpenZipFileName = null;

    public SearchForm()
    {
        InitializeComponent();

        tabsMain.DrawMode = TabDrawMode.OwnerDrawFixed;
        tabsMain.DrawItem += new DrawItemEventHandler(DrawTabHeaders);
    }

    private void mnuNewSearchTab_Click(object sender, EventArgs e)
    {
        AddNewTab(UserTabState.CreateDefault());
    }

    public SearchTabPage AddNewTab(UserTabState initialState, Color? initialHeaderColour = null)
    {
        if (initialState.Id == 0)
        {
            initialState.SearchDto.Id = _nextTabId++;
            initialState.TabSettings.Id = initialState.SearchDto.Id;
        }
        else
        {
            _nextTabId = Math.Max(_nextTabId, initialState.Id + 1);
        }

        var searchControl = SearchFormHelper.CreateSearchTabControl(initialState);
        var newTab = new SearchTabPage(this, searchControl, initialState.SearchDto.GetDisplayTitle(), initialHeaderColour);

        tabsMain.TabPages.Add(newTab); // second SearchTab_Resize call happens here
        newTab.ApplyUserPreferences(initialState.TabSettings);
        tabsMain.SelectedTab = newTab;
        newTab.InitialFocus();

        return newTab;
    }



    private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
    {
        CloseTab(tabsMain.SelectedIndex);
    }

    private void CloseTab(int index)
    {
        if (tabsMain.TabPages[index] is SearchTabPage selectedTabPage)
        {
            if (tabsMain.SelectedIndex == index)
            {
                tabsMain.SelectedIndex = index == 0 ? 1 : index - 1;
            }
            selectedTabPage.CancelSearch();
            tabsMain.TabPages.Remove(selectedTabPage);
            UserSearchRepository.DeleteSearchDto(selectedTabPage.Index);
            TabSettingsRepository.DeleteTabSettings(selectedTabPage.Index);
        }

        if (tabsMain.TabPages.Count == 0)
        {
            Application.Exit();
        }
    }

    private async void SearchForm_Load(object sender, EventArgs e)
    {
        if (Properties.Settings.Default.WindowState >= 0 && Properties.Settings.Default.WindowState <= 2)
        {
            this.WindowState = (FormWindowState)Properties.Settings.Default.WindowState;
        }

        if (WindowState == FormWindowState.Normal)
        {
            // Restore saved position and size
            if (Properties.Settings.Default.WindowSize.Width > 0 &&
                Properties.Settings.Default.WindowSize.Height > 0)
            {
                this.Size = Properties.Settings.Default.WindowSize;
            }

            if (Properties.Settings.Default.WindowLocation.X >= 0 &&
                Properties.Settings.Default.WindowLocation.Y >= 0)
            {
                // Ensure the saved location is visible on current monitors
                Rectangle savedBounds = new Rectangle(
                    Properties.Settings.Default.WindowLocation,
                    Properties.Settings.Default.WindowSize);

                if (SearchFormHelper.IsOnScreen(savedBounds))
                {
                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = Properties.Settings.Default.WindowLocation;
                }
                else
                {
                    // Fallback: center on primary screen
                    this.StartPosition = FormStartPosition.CenterScreen;
                }
            }
        }

        TextCriterionHistoryCollection.Instance.EnsureRefreshed();
        var savedTabs = UserSearchRepository.LoadSavedSearches();
        var savedPrefs = TabSettingsRepository.LoadSavedTabSettings();
        if (savedTabs.Length > 0)
        {
            RestoreSavedTabs(savedTabs, savedPrefs);
        }
        else if (_verifiedInitialOpenZipFileName == null)
        {
            _ = AddNewTab(UserTabState.CreateDefault());
        }

        if (_verifiedInitialOpenZipFileName != null)
        {
            await AddTabFromZipAsync(_verifiedInitialOpenZipFileName);
        }
    }

    private void RestoreSavedTabs(UserSearchDto[] savedTabs,
                                  UserTabSettings[] savedPrefs)
    {
        tabsMain.Visible = false;

        foreach (var savedTab in savedTabs)
        {
            var tabPrefs = savedPrefs.FirstOrDefault(p => p.Id == savedTab.Id) ?? UserTabSettings.CreateDefault();
            _ = AddNewTab(UserTabState.Create(savedTab, tabPrefs));
        }

        if (Properties.Settings.Default.LastTabIndex < 0
            || Properties.Settings.Default.LastTabIndex >= tabsMain.TabCount)
        {
            Properties.Settings.Default.LastTabIndex = 0;
        }

        tabsMain.SelectedIndex = Properties.Settings.Default.LastTabIndex;
        tabsMain.Visible = true;
    }

    private void DrawTabHeaders(object? sender, DrawItemEventArgs e)
    {
        SearchFormHelper.DrawTabHeaders(tabsMain, e);
    }

    private void duplicateTabToolStripMenuItem_Click(object sender, EventArgs e)
    {
        DuplicateTab(tabsMain.SelectedIndex);
    }

    private void DuplicateTab(int index)
    {
        if (tabsMain.TabPages[index] is SearchTabPage tabToDuplicate)
        {
            var tabSearchSettings = tabToDuplicate?.GetCurrentSearchSettings();
            if (tabSearchSettings != null)
            {
                tabSearchSettings.SearchDto.Id = 0; // Reset ID for the new tab
                AddNewTab(tabSearchSettings);
            }
        }
    }

    private void mnuQuit_Click(object sender, EventArgs e)
    {
        if (tabsMain.SelectedTab is SearchTabPage selectedTabPage)
        {
            selectedTabPage.CancelSearch();
        }

        Application.Exit();
    }

    private void mnuClearSearchHistory_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show("History will be removed from all dropdown lists.", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        if (result != DialogResult.OK)
        {
            return;
        }

        TextCriterionHistoryCollection.Instance.ClearAll();
        UserSearchRepository.DeleteHistoryFiles();

        foreach (SearchTabPage tabPage in tabsMain.TabPages)
        {
            tabPage.ClearHistory();
        }
    }

    internal void RedrawTabControl()
    {
        tabsMain.Invalidate();
    }

    private void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        Properties.Settings.Default.LastTabIndex = tabsMain.SelectedIndex;

        foreach (SearchTabPage tabPage in tabsMain.TabPages)
        {
            TabSettingsRepository.SaveTabSettings(tabPage.GetCurrentUserSettings());
        }

        Properties.Settings.Default.WindowState = (int)WindowState;

        // Save current size and location
        if (this.WindowState == FormWindowState.Normal)
        {
            Properties.Settings.Default.WindowSize = this.Size;
            Properties.Settings.Default.WindowLocation = this.Location;
        }
        else
        {
            // Save restored bounds if maximized/minimized
            Properties.Settings.Default.WindowSize = this.RestoreBounds.Size;
            Properties.Settings.Default.WindowLocation = this.RestoreBounds.Location;
        }

        Properties.Settings.Default.Save();
    }



    private void mnuCloseTab_Click(object sender, EventArgs e)
    {
        if (_tabIndexForContextMenu.HasValue)
        {
            CloseTab(_tabIndexForContextMenu.Value);
        }
    }

    private void mnuDuplicateTab_Click(object sender, EventArgs e)
    {
        if (_tabIndexForContextMenu.HasValue)
        {
            DuplicateTab(_tabIndexForContextMenu.Value);
        }
    }

    private void mnuCloseOtherTabs_Click(object sender, EventArgs e)
    {
        if (!_tabIndexForContextMenu.HasValue)
            return;

        var tabToKeep = tabsMain.TabPages[_tabIndexForContextMenu.Value];

        while (tabsMain.TabPages.Count > 1)
        {
            var lastTabIndex = tabsMain.TabPages.Count - 1;
            if (tabsMain.TabPages[lastTabIndex] == tabToKeep)
                lastTabIndex--;
            CloseTab(lastTabIndex);
        }
    }

    private int? _tabIndexForContextMenu = null;

    private void mnuTabContext_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _tabIndexForContextMenu = null;

        Point p = this.tabsMain.PointToClient(Cursor.Position);
        for (int i = 0; i < this.tabsMain.TabCount; i++)
        {
            Rectangle r = this.tabsMain.GetTabRect(i);
            if (r.Contains(p))
            {
                _tabIndexForContextMenu = i; // i is the index of tab under cursor
                return;
            }
        }
        e.Cancel = true;
    }

    private async void mnuExploreZip_Click(object sender, EventArgs e)
    {
        string? zipFileName = null;
        using OpenFileDialog dialog = new();

        dialog.Title = "Select a ZIP file";
        dialog.Filter = "ZIP files (*.zip)|*.zip|All files (*.*)|*.*";
        dialog.InitialDirectory = SearchFormHelper.GetLastZipLocation();
        dialog.CheckFileExists = true;
        dialog.CheckPathExists = true;

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            Properties.Settings.Default.LastZipLocation = Path.GetDirectoryName(dialog.FileName);
            zipFileName = dialog.FileName;
        }
        else
        {
            return;
        }

        bool flowControl = await AddTabFromZipAsync(zipFileName);
        if (!flowControl)
        {
            return;
        }
    }

    private async Task<bool> AddTabFromZipAsync(string zipFileName)
    {
        var unzipProgressWindow = new ProgressWindow(zipFileName);
        var result = unzipProgressWindow.ShowDialog();
        if (result == DialogResult.OK)
        {
            var tempDir = unzipProgressWindow.ExtractedDirectoryPath;
            AddZipTab(tempDir!);
            return true;
        }

        return false;
    }

    private void AddZipTab(string tempDir)
    {
        var zipExploreUserSearchDto = new UserSearchDto()
        {
            Id = 0,
            RawIncludeFilesPattern = null,
            RawExcludeFilesPattern = null,
            BaseDirectoriesPattern = tempDir
        };

        var zipExploreState = new UserTabState(zipExploreUserSearchDto, UserTabSettings.CreateDefault());
        AddNewTab(zipExploreState, Color.Lavender);
    }

    private string? _verifiedDropFileName = null;
    private string? _currentUnverifiedDropFileName = null;

    private void SearchForm_DragEnter(object sender, DragEventArgs e)
    {
        Debug.WriteLine("in DragEnter");
        var unverifiedFileName = SearchFormHelper.GetRawDraggedFileName(e);

        if (unverifiedFileName == null)
        {
            return; // Not at all a valid file.
        }

        if (_currentUnverifiedDropFileName != null
            && unverifiedFileName == _currentUnverifiedDropFileName)
        {
            return; // We've already started this drag.
        }

        if (_currentUnverifiedDropFileName != null
            && unverifiedFileName != _currentUnverifiedDropFileName)
        {
            _verifiedDropFileName = null;
            _currentUnverifiedDropFileName = null;
            return; // Wow, that's weird.  Abandon whatever drag we were doing.
        }

        _verifiedDropFileName = null;
        _currentUnverifiedDropFileName = null;

        var verifiedFileName = SearchFormHelper.GetDraggedZipFileName(unverifiedFileName);
        if (verifiedFileName != null)
        {
            e.Effect = DragDropEffects.Copy;
            _verifiedDropFileName = verifiedFileName;
            _currentUnverifiedDropFileName = unverifiedFileName;
            Debug.WriteLine("Set Copy");
        }
        else
        {
            Debug.WriteLine("Set none");
            _currentUnverifiedDropFileName = null;
            e.Effect = DragDropEffects.None;
        }
    }

    private async void SearchForm_DragDrop(object sender, DragEventArgs e)
    {
        Debug.WriteLine("in DragDrop");
        _currentUnverifiedDropFileName = null;

        if (_verifiedDropFileName != null)
        {
            var dropFileName = _verifiedDropFileName;
            _verifiedDropFileName = null;
            await AddTabFromZipAsync(dropFileName);
        }
    }

    private void tabsMain_DragEnter(object sender, DragEventArgs e)
    {
        SearchForm_DragEnter(sender, e);
    }

    private void tabsMain_DragDrop(object sender, DragEventArgs e)
    {
        SearchForm_DragDrop(sender, e);
    }

    private void tabsMain_DragLeave(object sender, EventArgs e)
    {
        SearchForm_DragLeave(sender, e);
    }

    private void SearchForm_DragLeave(object sender, EventArgs e)
    {
        Debug.WriteLine("In DragLeave");
        _currentUnverifiedDropFileName = null;
        _verifiedDropFileName = null;
    }
}