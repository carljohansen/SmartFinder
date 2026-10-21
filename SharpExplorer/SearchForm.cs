using SharpExplorer.Domain.History;

namespace SharpExplorer;

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
        AddNewTab(UserTabState.Default);
    }

    private SearchTabPage AddNewTab(UserTabState initialState)
    {
        if (initialState.SearchDto.Id == 0)
        {
            initialState.SearchDto.Id = _nextTabId++;
        }
        else
        {
            _nextTabId = Math.Max(_nextTabId, initialState.SearchDto.Id + 1);
        }


        var searchControl = CreateSearchTabControl(initialState);
        var newTab = new SearchTabPage(this, searchControl, initialState.SearchDto.GetDisplayTitle());

        tabsMain.TabPages.Add(newTab);
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
        if (tabsMain.SelectedTab is SearchTabPage selectedTabPage)
        {
            tabsMain.SelectedIndex = tabsMain.SelectedIndex == 0 ? 1 : tabsMain.SelectedIndex - 1;
            selectedTabPage.CancelSearch();
            tabsMain.TabPages.Remove(selectedTabPage);
            UserSearchRepository.DeleteSearchDto(selectedTabPage.Index);
        }

        if (tabsMain.TabPages.Count == 0)
        {
            Application.Exit();
        }
    }

    private void SearchForm_Load(object sender, EventArgs e)
    {
        var savedTabs = UserSearchRepository.LoadSavedSearches();
        if (savedTabs.Length > 0)
        {
            foreach (var savedTab in savedTabs)
            {
                _ = AddNewTab(new(savedTab, UserTabSettings.Default));
            }
            tabsMain.SelectedIndex = 0;
        }
        else
        {
            _ = AddNewTab(UserTabState.Default);
        }
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
        var currentTab = tabsMain.SelectedTab as SearchTabPage;
        var currentTabSearchSettings = currentTab?.GetCurrentSearchSettings();
        if (currentTabSearchSettings != null)
        {
            currentTabSearchSettings.SearchDto.Id = 0; // Reset ID for the new tab
            AddNewTab(currentTabSearchSettings);
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
}
