using SharpExplorer.Domain;
using SharpExplorer.Domain.TextSearch;
using System.ComponentModel;
using System.Diagnostics;

namespace SharpExplorer
{
    public partial class Form1 : Form
    {
        private readonly Brush[] brushes = [.. new[] { Color.Black, Color.Orange }.Select(c => new SolidBrush(c))];
        private readonly StringFormat _formatForMeasuring;
        private FileSearch _currentSearch;
        private readonly BindingList<FileDisplay> _boundItems;
        private readonly BindingSource _searchResultDataSource;
        private int contextMenuRowIndex = -1;

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

        private UserSearchDto CreateUserSearchDto()
        {
            var searchPatterns = txtFilePattern.Text;
            var exceptSearchPatterns = txtExceptFilePattern.Text;
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
                RawExcludeFilesPattern = exceptSearchPatterns,
                TextSearchPattern = query,
                TextMatchingOptions = options,
                BaseDirectoriesPattern = directoryPath
            };
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            var searchDto = CreateUserSearchDto();
            var searchPatternCollection = PathCollection.Create(searchDto.RawIncludeFilesPattern);
            var excludeSearchPatternCollection = PathCollection.Create(searchDto.RawExcludeFilesPattern);

            _boundItems.Clear();

            _currentSearch = new FileSearch();
            if (searchDto.HasTextSearch())
            {
                var matchFragments = searchDto.ParseTextSearchPattern();
                _currentSearch.TextSearches.AddRange(matchFragments);
            }

            await foreach (var item in _currentSearch.SearchFilesAsync(searchDto.BaseDirectoriesPattern, searchPatternCollection, excludeSearchPatternCollection))
            {
                item.InitializeMetadata();
                _boundItems.Add(item);
            }

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

        private void mnuShowInExplorer_Click(object sender, EventArgs e)
        {
            if (contextMenuRowIndex < 0)
            {
                return;
            }

            var selectedRow = grdResult.Rows[contextMenuRowIndex];
            var selectedFilePath = selectedRow.DataBoundItem is FileDisplay fileDisplay ? fileDisplay.FullPath : null;
            if (selectedFilePath == null
                || !File.Exists(selectedFilePath))
            {
                return;
            }

            var containingFolder = Path.GetDirectoryName(selectedFilePath);
            if (containingFolder != null)
            {
                Process.Start("explorer.exe", containingFolder);
            }
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
            ColumnHeader header = new()
            {
                Text = "",
                Name = "col1",
                Width = 2000
            };
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

        private void txtFilePattern_Enter(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void txtExceptFilePattern_Enter(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void txtRootDir_Enter(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

    }
}
