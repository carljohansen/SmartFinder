using SharpExplorer.Domain;
using SharpExplorer.Domain.TextSearch;

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

        private void AddNewTab(UserSearchDto? initialSearchSettings)
        {
            var searchControl = CreateSearchTabControl(initialSearchSettings);
            var newTab = new SearchTabPage(this, searchControl) { Text = $"Search {_newTabCounter++}" };

            tabsMain.TabPages.Add(newTab);
            tabsMain.SelectedTab = newTab;
        }

        private SearchTab CreateSearchTabControl(UserSearchDto? initialSearchControlState) => new(initialSearchControlState)
        {
            Left = 0,
            Top = 0,
            Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
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

            AddNewTab(lastSearch);
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
