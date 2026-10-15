namespace SharpExplorer
{
    public partial class SearchForm : Form
    {
        public SearchForm()
        {
            InitializeComponent();
        }

        public void SetStatusText(string text)
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
            var newTab = new TabPage() { Text = "New 1" };
            tabsMain.TabPages.Add(newTab);

            InitializeSearchTab(newTab);
            tabsMain.SelectedTab = newTab;
        }

        private void InitializeSearchTab(TabPage searchTabPage)
        {
            var newSearch = new SearchTab();
            searchTabPage.Controls.Add(newSearch);

            newSearch.Left = 0;
            newSearch.Top = 0;
            newSearch.Width = searchTabPage.Width;
            newSearch.Height = searchTabPage.Height;
            newSearch.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabsMain.TabPages.Count == 1)
            {
                Application.Exit();
            }

            var selectedTab = tabsMain.SelectedTab;
            tabsMain.SelectedIndex = tabsMain.SelectedIndex == 0 ? 1 : tabsMain.SelectedIndex - 1;
            tabsMain.TabPages.Remove(selectedTab!);
        }

        private void SearchForm_Load(object sender, EventArgs e)
        {
            if (tabsMain.TabCount == 0)
            {
                return;
            }

            var initialTab = tabsMain.TabPages[0];
            InitializeSearchTab(initialTab);
        }
    }
}
