using SharpExplorer.Domain;
using SharpExplorer.Domain.TextSearch;
using System.ComponentModel;

namespace SharpExplorer
{
    public partial class Form1 : Form
    {
        private readonly Brush[] brushes = new[] { Color.Black, Color.Orange }.Select(c => new SolidBrush(c)).ToArray();
        private readonly StringFormat _formatForMeasuring;
        public Form1()
        {
            InitializeComponent();
            _formatForMeasuring = new(StringFormat.GenericTypographic)
            {
                Alignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };
            _formatForMeasuring.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            grdResult.AutoGenerateColumns = false;
            _boundItems = [];
            _searchResultDataSource = new BindingSource(_boundItems, null!);
        }

        private FileSearch _currentSearch;

        private UserSearchDto CreateUserSearchDto()
        {
            var searchPatterns = txtFilePattern.Text;
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

            return new UserSearchDto
            {
                RawIncludeFilesPattern = searchPatterns,
                RawExcludeFilesPattern = string.Empty,
                TextSearchPattern = query,
                TextMatchingOptions = options,
                BaseDirectoriesPattern = directoryPath
            };
        }

        private readonly BindingList<FileDisplay> _boundItems;
        private readonly BindingSource _searchResultDataSource;

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            var searchDto = CreateUserSearchDto();
            var searchPatternCollection = PathCollection.Create(searchDto.RawIncludeFilesPattern);

            _boundItems.Clear();

            _currentSearch = new FileSearch();
            if (searchDto.HasTextSearch())
            {
                _currentSearch.TextSearches.Add(new AtomicTextMatch(searchDto.TextSearchPattern, searchDto.TextMatchingOptions));
            }

            foreach (var searchPattern in searchPatternCollection.Paths)
            {
                await foreach (var item in _currentSearch.SearchFilesAsync(searchDto.BaseDirectoriesPattern, searchPattern))
                {
                    item.InitializeMetadata();
                    _boundItems.Add(item);
                }
            }

            //foreach (var searchPattern in searchPatternCollection.Paths)
            //{
            //    await foreach (var item in _currentSearch.SearchFilesAsync(searchDto.BaseDirectoriesPattern, searchPattern))
            //    {
            //        item.InitializeMetadata();
            //        var newRowIndex = grdResult.Rows.Add(item.Name, item.Location, item.SizeDisplay, item.LastModified);
            //        grdResult.Rows[newRowIndex].Tag = item;
            //    }
            //}

            //if (grdResult.SortedColumn != null)
            //{
            //    grdResult.Sort(grdResult.SortedColumn, grdResult.SortOrder == SortOrder.Ascending ? System.ComponentModel.ListSortDirection.Ascending : System.ComponentModel.ListSortDirection.Descending);
            //}

            //_searchResultDataSource.Sort = $"Name DESC";

            if (grdResult.Rows.Count > 0)
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
            var selectedFilePath = selectedRow.DataBoundItem is FileDisplay fileDisplay ? fileDisplay.FullPath : null;
            if (selectedFilePath != null)
            {
                Clipboard.SetText($"\"{selectedFilePath}\"");
            }
        }

        private int contextMenuRowIndex = -1;

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
            using var highlightFont = new Font(font.FontFamily, font.Size, FontStyle.Bold, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
            float xPos = e.Bounds.Left;

            using Graphics g = e.Graphics;

            foreach (var segment in textSegments)
            {
                string segmentText = segment.SegmentText;
                Brush brush = brushes[segment.SegmentColourIndex];
                var segmentFont = segment.SegmentColourIndex == 0 ? font : highlightFont;
                g.DrawString(segmentText, segmentFont, brush, xPos, e.Bounds.Top, StringFormat.GenericTypographic);
                xPos += g.MeasureString(segmentText, segmentFont, 0, _formatForMeasuring).Width;
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
            if (selectedRow.DataBoundItem is FileDisplay fileDisplay)
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
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lstTextMatches.View = View.Details;
            ColumnHeader header = new()
            {
                Text = "",
                Name = "col1",
                Width = 2000
            };
            lstTextMatches.HeaderStyle = ColumnHeaderStyle.None;
            lstTextMatches.Columns.Add(header);

            grdResult.DataSource = _searchResultDataSource;
            grdResult.Columns["colName"]!.DataPropertyName = nameof(FileDisplay.Name);
            grdResult.Columns["colLocation"]!.DataPropertyName = nameof(FileDisplay.Location);
            grdResult.Columns["colSize"]!.DataPropertyName = nameof(FileDisplay.SizeDisplay);
            grdResult.Columns["colLastModified"]!.DataPropertyName = nameof(FileDisplay.LastModified);
        }

        private void grdResult_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            contextMenuRowIndex = e.RowIndex;
        }
    }
}
