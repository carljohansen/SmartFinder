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
            var newSearch = new SearchTab();
            newTab.Controls.Add(newSearch);

            tabsMain.TabPages.Add(newTab);

            newSearch.Left = 0;
            newSearch.Top = 0;
            newSearch.Width = newTab.Width;
            newSearch.Height = newTab.Height;

            tabsMain.SelectedTab = newTab;
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
    }
}
