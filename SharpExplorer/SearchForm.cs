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
    }
}
