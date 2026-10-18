using SharpExplorer.Domain;

namespace SharpExplorer
{
    public partial class SearchForm : Form
    {
        private int _newTabCounter = 1;

        public SearchForm()
        {
            InitializeComponent();
        }

        public void SetStatusText(string? text)
        {
            lblFormStatusMessage.Text = text;
        }

        public void SetProgress(int progressPercentage)
        {
            MainProgressBar.Visible = true;
            MainProgressBar.Value = Math.Min(Math.Max(progressPercentage, 0), 100);
        }

        public void HideProgressBar()
        {
            MainProgressBar.Visible = false;
        }

        private void mnuNewSearchTab_Click(object sender, EventArgs e)
        {
            AddNewTab(UserSearchDto.Default);
        }

        private SearchTabPage AddNewTab(UserSearchDto? initialSearchSettings)
        {
            var searchControl = CreateSearchTabControl(initialSearchSettings);
            var newTab = new SearchTabPage(this, searchControl) { Text = $"Search {_newTabCounter++}" };

            tabsMain.TabPages.Add(newTab);
            tabsMain.SelectedTab = newTab;
            return newTab;
        }

        private SearchTab CreateSearchTabControl(UserSearchDto? initialSearchControlState) => new(initialSearchControlState)
        {
            Dock = DockStyle.Fill
        };

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabsMain.TabPages.Count == 1)
            {
                return; // Can't close the last tab
            }

            if (tabsMain.SelectedTab is SearchTabPage selectedTabPage)
            {
                tabsMain.SelectedIndex = tabsMain.SelectedIndex == 0 ? 1 : tabsMain.SelectedIndex - 1;
                selectedTabPage.CancelSearch();
                tabsMain.TabPages.Remove(selectedTabPage);
            }
        }

        private void SearchForm_Load(object sender, EventArgs e)
        {
            var lastSearch = UserSearchRepository.GetLastSearchDto();
            _ = AddNewTab(lastSearch);
            // HACK: this is my workaround for the bug where right-anchored combobox is not initially drawn.
            this.Width += 1;
            this.Width -= 1;
        }

        private void duplicateTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var currentTab = tabsMain.SelectedTab as SearchTabPage;
            var currentTabSearchSettings = currentTab?.GetCurrentSearchSettings();
            if (currentTabSearchSettings != null)
            {
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
    }
}
