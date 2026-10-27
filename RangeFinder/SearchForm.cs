using RangeFinder.Domain;
using RangeFinder.Domain.History;

namespace RangeFinder;

public partial class SearchForm : Form
{
    private int _nextTabId = 1;

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

    public SearchTabPage AddNewTab(UserTabState initialState)
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

        var searchControl = CreateSearchTabControl(initialState);
        var newTab = new SearchTabPage(this, searchControl, initialState.SearchDto.GetDisplayTitle());

        tabsMain.TabPages.Add(newTab); // second SearchTab_Resize call happens here
        newTab.ApplyUserPreferences(initialState.TabSettings);
        tabsMain.SelectedTab = newTab;
        newTab.InitialFocus();

        return newTab;
    }

    private static SearchTab CreateSearchTabControl(UserTabState initialState) => new(initialState)
    {
        Dock = DockStyle.Fill
    };

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

    private void SearchForm_Load(object sender, EventArgs e)
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

                if (IsOnScreen(savedBounds))
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
        else
        {
            _ = AddNewTab(UserTabState.CreateDefault());
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
        //e.DrawBackground();
        var tabPage = tabsMain.TabPages[e.Index] as SearchTabPage;
        using Brush br = new SolidBrush(tabPage?.HeaderColor ?? default);
        e.Graphics.FillRectangle(br, e.Bounds);
        SizeF sz = e.Graphics.MeasureString(tabsMain.TabPages[e.Index].Text, e.Font!);
        e.Graphics.DrawString(tabsMain.TabPages[e.Index].Text, e.Font!, Brushes.Black, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1);

        if (e.Index != tabsMain.SelectedIndex)
        {
            return;
        }

        Rectangle rect = e.Bounds;
        rect.Offset(0, 1);
        rect.Inflate(0, -1);
        e.Graphics.DrawRectangle(Pens.DarkGray, rect);
        e.DrawFocusRectangle();
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

    private bool IsOnScreen(Rectangle rect)
    {
        foreach (Screen screen in Screen.AllScreens)
        {
            if (screen.WorkingArea.IntersectsWith(rect))
                return true;
        }
        return false;
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
        dialog.Filter = "ZIP files (*.zip)|*.zip";
        dialog.InitialDirectory = GetLastZipLocation();
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

        var tempDir = await ZipExpander.Instance.UnzipToTempDirectoryAsync(zipFileName, CancellationToken.None);
        if (tempDir == null)
        {
            return;
        }

        var zipExploreUserSearchDto = new UserSearchDto()
        {
            Id = 0,
            RawIncludeFilesPattern = null,
            RawExcludeFilesPattern = null,
            BaseDirectoriesPattern = tempDir
        };

        var zipExploreState = new UserTabState(zipExploreUserSearchDto, UserTabSettings.CreateDefault());
        AddNewTab(zipExploreState);
    }

    private static string GetLastZipLocation()
    {
        var result = Properties.Settings.Default.LastZipLocation;
        if (string.IsNullOrWhiteSpace(result) || !Directory.Exists(result))
        {
            result = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
        return result;
    }
}