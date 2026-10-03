using SharpExplorer.Domain;

namespace SharpExplorer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            var searchPattern = txtFilePattern.Text;
            var directoryPath = txtRootDir.Text;
            grdResult.Rows.Clear();

            var search = new FileSearch();

            await foreach (var item in search.SearchFilesAsync(directoryPath, searchPattern))
            {
                grdResult.Rows.Add(item.Name, item.Location);
            }
        }
    }
}
