using SharpExplorer.Domain;
using SharpExplorer.Domain.TextSearch;

namespace SharpExplorer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private FileSearch _currentSearch;

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            var searchPattern = txtFilePattern.Text;
            var directoryPath = txtRootDir.Text;
            var query = txtContainingText.Text.Trim();

            grdResult.Rows.Clear();

            _currentSearch = new FileSearch();
            if (!string.IsNullOrWhiteSpace(query))
            {
                _currentSearch.TextSearches.Add(new AtomicTextMatch(query, TextMatchingOptions.None));
            }

            await foreach (var item in _currentSearch.SearchFilesAsync(directoryPath, searchPattern))
            {
                item.InitializeMetadata();
                var newRowIndex = grdResult.Rows.Add(item.Name, item.Location, item.SizeDisplay, item.LastModified);
                grdResult.Rows[newRowIndex].Tag = item;
            }
        }

        private void grdResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var selectedRow = grdResult.Rows[e.RowIndex];
            if (selectedRow.Tag is FileDisplay fileDisplay)
            {
                var matches = _currentSearch.GetMatches(fileDisplay.FullPath);
            }
        }
    }
}
