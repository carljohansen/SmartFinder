using SharpExplorer.Domain;
using SharpExplorer.Domain.TextSearch;

namespace SharpExplorer
{
    public partial class Form1 : Form
    {
        private readonly Brush[] brushes = new[] { Color.Black, Color.Orange }.Select(c => new SolidBrush(c)).ToArray();

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
            var doMatchCase = chkMatchCase.Checked;
            var doMatchWholeWord = chkMatchWholeWord.Checked;
            var options = TextMatchingOptions.None;
            if (doMatchCase)
            {
                options |= TextMatchingOptions.CaseSensitive;
            }
            if (doMatchWholeWord)
            {
                options |= TextMatchingOptions.WholeWord;
            }
            grdResult.Rows.Clear();

            _currentSearch = new FileSearch();
            if (!string.IsNullOrWhiteSpace(query))
            {
                _currentSearch.TextSearches.Add(new AtomicTextMatch(query, options));
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

            if(grdResult.Rows.Count > 0)
            {
                grdResult.Rows[0].Selected = true;
                grdResult.CurrentCell = grdResult.Rows[0].Cells[0];
                grdResult.Focus();
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

        private void mnuCopyPath_Click(object sender, EventArgs e)
        {
            if (contextMenuRowIndex < 0)
            {
                return;
            }

            var selectedRow = grdResult.Rows[contextMenuRowIndex];
            var selectedFilePath = selectedRow.Tag is FileDisplay fileDisplay ? fileDisplay.FullPath : null;
            if (selectedFilePath != null)
            {
                Clipboard.SetText(selectedFilePath);
            }
        }

        private int contextMenuRowIndex = -1;



        private void grdResult_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            var x = e.RowIndex;
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void grdResult_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void lstTextMatches_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (e.ItemIndex < 0) return;

            var itemUnderlyingMatches = lstTextMatches.Items[e.ItemIndex].Tag as FileLineTextMatch;

            if (itemUnderlyingMatches == null)
            {
                return;
            }

            var textSegments = itemUnderlyingMatches.GetSegments();

            e.DrawBackground();

            Font font = lstTextMatches.Font;
            float xPos = e.Bounds.Left;

            using Graphics g = e.Graphics;

            foreach (var segment in textSegments)
            {
                string segmentText = segment.SegmentText;
                Brush brush = brushes[segment.SegmentColourIndex];
                g.DrawString(segmentText, font, brush, xPos, e.Bounds.Top);
                xPos += g.MeasureString(segmentText, font).Width;
            }

            e.DrawFocusRectangle();
        }

        private void grdResult_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            var selectedRow = grdResult.Rows[e.RowIndex];
            if (selectedRow.Tag is FileDisplay fileDisplay)
            {
                var matches = _currentSearch.GetMatches(fileDisplay.FullPath);
                lstTextMatches.Items.Clear();
                foreach (var match in matches)
                {
                    var newItem = lstTextMatches.Items.Add(match.Line);
                    newItem.Tag = match;
                }
            }
        }

        private void txtContainingText_Enter(object sender, EventArgs e)
        {
            if(sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }
    }
}
