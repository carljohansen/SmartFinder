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
            var searchPatterns = txtFilePattern.Text;
            var searchPatternCollection = PathCollection.Create(searchPatterns);
            var directoryPath = txtRootDir.Text;
            var query = txtContainingText.Text.Trim();

            grdResult.Rows.Clear();

            _currentSearch = new FileSearch();
            if (!string.IsNullOrWhiteSpace(query))
            {
                _currentSearch.TextSearches.Add(new AtomicTextMatch(query, TextMatchingOptions.None));
            }

            foreach (var searchPattern in searchPatternCollection.Paths)
            {
                await foreach (var item in _currentSearch.SearchFilesAsync(directoryPath, searchPattern))
                {
                    item.InitializeMetadata();
                    var newRowIndex = grdResult.Rows.Add(item.Name, item.Location, item.SizeDisplay, item.LastModified);
                    grdResult.Rows[newRowIndex].Tag = item;
                }
            }

            if (grdResult.SortedColumn != null)
            {
                grdResult.Sort(grdResult.SortedColumn, grdResult.SortOrder == SortOrder.Ascending ? System.ComponentModel.ListSortDirection.Ascending : System.ComponentModel.ListSortDirection.Descending);
            }
        }

        private void grdResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            var selectedRow = grdResult.Rows[e.RowIndex];
            if (selectedRow.Tag is FileDisplay fileDisplay)
            {
                var matches = _currentSearch.GetMatches(fileDisplay.FullPath);
            }
        }

        private void grdResult_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column == grdResult.Columns["colSize"])
            {
                e.Handled = true;
                var size1 = grdResult.Rows[e.RowIndex1].Tag is FileDisplay file1 ? file1.Size : 0;
                var size2 = grdResult.Rows[e.RowIndex2].Tag is FileDisplay file2 ? file2.Size : 0;
                e.SortResult = size1 < size2 ? -1 : (size1 > size2 ? 1 : 0);
            }
        }
    }
}
