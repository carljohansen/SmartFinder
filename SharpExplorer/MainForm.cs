using SharpExplorer.Domain;
using SharpExplorer.Domain.FileType;
using SharpExplorer.Domain.TextSearch;
using System.Data;
using System.Diagnostics;

namespace SharpExplorer
{
    public partial class MainForm : Form
    {
        private readonly FileSearch _currentSearch;

        private readonly Brush[] _brushes = [.. new[] { Color.Black, Color.Orange, Color.Purple, Color.CornflowerBlue, Color.DarkKhaki, Color.Green }.Select(c => new SolidBrush(c))];
        private readonly StringFormat _formatForMeasuring;

        private int contextMenuRowIndex = -1;
        private CancellationTokenSource? _searchCancellationTokenSource;

        public MainForm()
        {
            InitializeComponent();

            var resultTable = SearchResultDataTableHelper.CreateFileResultDataTableColumns();
            var searchResultDataSource = new BindingSource(resultTable.DefaultView, null!);
            _currentSearch = new(resultTable);

            grdResult.AutoGenerateColumns = false;
            grdResult.DataSource = searchResultDataSource;

            _formatForMeasuring = new(StringFormat.GenericTypographic)
            {
                Alignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };
            _formatForMeasuring.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            _currentSearch.SearchProgressUpdate += HandleSearchStatusUpdate;
        }

        private UserSearchDto CreateUserSearchDto()
        {
            var searchPatterns = txtFilePattern.Text;
            var exceptSearchPatterns = txtExceptFilePattern.Text;
            var directoryPath = string.IsNullOrEmpty(txtRootDir.Text) ? "." : txtRootDir.Text;
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

            float? userSearchDateFilterQty = null;
            if (float.TryParse(txtDateFilterAmount.Text, out var dateFilterQty))
            {
                userSearchDateFilterQty = dateFilterQty;
            }

            return new UserSearchDto
            {
                RawIncludeFilesPattern = searchPatterns,
                RawExcludeFilesPattern = exceptSearchPatterns,
                TextSearchPattern = query,
                TextMatchingOptions = options,
                BaseDirectoriesPattern = directoryPath,
                FilterByModifiedDate = chkFilterByDate.Checked,
                ModifiedInLastQuantity = userSearchDateFilterQty,
                ModifiedInLastQuantityType = cmbDateFilterAmountType.SelectedItem?.ToString(),
            };
        }

        private void ApplyUserSearchDto(UserSearchDto dto)
        {
            txtFilePattern.Text = dto.RawIncludeFilesPattern ?? "";
            txtExceptFilePattern.Text = dto.RawExcludeFilesPattern ?? "";
            txtContainingText.Text = dto.TextSearchPattern ?? "";
            txtRootDir.Text = dto.BaseDirectoriesPattern ?? "";
            chkMatchCase.Checked = dto.TextMatchingOptions.HasFlag(TextMatchingOptions.CaseSensitive);
            chkMatchWholeWord.Checked = dto.TextMatchingOptions.HasFlag(TextMatchingOptions.WholeWord);
            chkFilterByDate.Checked = dto.FilterByModifiedDate;
            txtDateFilterAmount.Text = dto.ModifiedInLastQuantity?.ToString() ?? "";
            if (!string.IsNullOrEmpty(dto.ModifiedInLastQuantityType))
            {
                cmbDateFilterAmountType.SelectedItem = dto.ModifiedInLastQuantityType;
            }
        }

        private FileDisplay? GetRowFileDisplay(int rowIndex)
        {
            if (rowIndex < 0)
                return null;
            var selectedRow = grdResult.Rows[rowIndex];
            return (selectedRow.DataBoundItem is DataRowView view && view.Row["FileDisplay"] is FileDisplay fileDisplay) ? fileDisplay : null;
        }

        private void HandleSearchStatusUpdate(object? sender, SearchProgressEventArgs e)
        {
            SetStatusText(e.StatusMessage);
        }

        private void SetStatusText(string text)
        {
            lblStatus.Text = text;
        }

        private void ClearStatusText() => SetStatusText("");

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            SetStatusText("Searching...");
            btnSearch.Enabled = false;
            btnCancelSearch.Enabled = true;
            lstTextMatches.Items.Clear();
            contextMenuRowIndex = -1;
            _searchCancellationTokenSource = new CancellationTokenSource();

            var searchDto = CreateUserSearchDto();

            try
            {
                var finalStatusMsg = await _currentSearch.SearchAndPopulateTableAsync(searchDto, _searchCancellationTokenSource.Token);
                SetStatusText(finalStatusMsg);
            }
            catch
            {
                SetStatusText("Invalid search criteria");
            }

            UserSearchRepository.SaveLastSearchDto(searchDto);

            var numMatches = grdResult.Rows.Count;
            if (numMatches > 0)
            {
                grdResult.Rows[0].Selected = true;
                grdResult.CurrentCell = grdResult.Rows[0].Cells[0];
                grdResult.Focus();

                spltResultPanel.FixedPanel = searchDto.HasTextSearch() ? FixedPanel.Panel1 : FixedPanel.Panel2;
            }

            btnSearch.Enabled = true;
            btnCancelSearch.Enabled = false;
        }

        private void mnuCopyPath_Click(object sender, EventArgs e)
        {
            var selectedFileDisplay = GetRowFileDisplay(contextMenuRowIndex);
            if (selectedFileDisplay == null)
                return;

            var selectedFilePath = selectedFileDisplay.FullPath;
            if (selectedFilePath != null)
            {
                Clipboard.SetText($"\"{selectedFilePath}\"");
            }
        }

        private void mnuShowInExplorer_Click(object sender, EventArgs e)
        {
            var selectedFileDisplay = GetRowFileDisplay(contextMenuRowIndex);
            if (selectedFileDisplay == null)
                return;

            var selectedFilePath = selectedFileDisplay.FullPath;
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

        private void mnuOpenWithTextEditor_Click(object sender, EventArgs e)
        {
            var selectedFileDisplay = GetRowFileDisplay(contextMenuRowIndex);
            if (selectedFileDisplay == null)
                return;

            var isDefinitelyTextFile = FileTypeHelper.IsKnownTextFile(selectedFileDisplay.GetFileExtension());
            if (!isDefinitelyTextFile)
            {
                return;
            }

            var textEditorExeName = FileExtensionHelper.GetFileExtensionInfo(AssocStr.Executable, ".txt");
            if (textEditorExeName != null)
            {
                Process.Start(textEditorExeName, selectedFileDisplay.FullPath);
            }
        }

        private void mnuOpenFile_Click(object sender, EventArgs e)
        {
            _ = OpenFileInRegisteredApp(contextMenuRowIndex);
        }

        private bool OpenFileInRegisteredApp(int rowIndex)
        {
            var selectedFileDisplay = GetRowFileDisplay(rowIndex);
            if (selectedFileDisplay == null)
                return false;

            Process.Start(new ProcessStartInfo
            {
                FileName = selectedFileDisplay.FullPath,
                UseShellExecute = true,
                Verb = "open"
            });
            return true;
        }

        private void grdResult_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            _ = OpenFileInRegisteredApp(e.RowIndex);
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

            // Line Number:
            string lineNumberText = itemUnderlyingMatches.LineNumber.ToString();
            Brush brush = _brushes[0];
            g.DrawString(lineNumberText, highlightFont, brush, xPos, e.Bounds.Top, StringFormat.GenericTypographic);
            xPos = e.Bounds.Left + 50; // Prepare for next col.

            // Text:
            foreach (var segment in textSegments)
            {
                string segmentText = segment.SegmentText;
                brush = _brushes[Math.Min(segment.SegmentColourIndex, _brushes.Length - 1)];
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

            var fileDisplay = GetRowFileDisplay(e.RowIndex);
            if (fileDisplay != null)
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

            var lastSearch = UserSearchRepository.GetLastSearchDto();
            if (lastSearch != null)
            {
                ApplyUserSearchDto(lastSearch);
            }

            EnableDisableControls();
        }

        private void grdResult_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            contextMenuRowIndex = e.RowIndex;

            var selectedRow = grdResult.Rows[contextMenuRowIndex];

            var enableOpenWithTextEditor = false;
            if (selectedRow.DataBoundItem is FileDisplay selectedFileDisplay
                && FileTypeHelper.IsKnownTextFile(selectedFileDisplay.GetFileExtension()))
            {
                enableOpenWithTextEditor = true;
            }

            mnuOpenWithTextEditor.Enabled = enableOpenWithTextEditor;
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

        private void txtDateFilterAmount_Enter(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void chkEnableExclusions_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableExclusions();
        }

        private void EnableDisableExclusions()
        {
            txtExceptFilePattern.Enabled = chkEnableExclusions.Checked;
        }

        private void EnableDisableDateFiltering()
        {
            var isEnabled = chkFilterByDate.Checked;
            txtDateFilterAmount.Enabled = isEnabled;
            cmbDateFilterAmountType.Enabled = isEnabled;
            chkFilterByDate.ForeColor = isEnabled ? SystemColors.ControlText : SystemColors.GrayText;
        }

        private void EnableDisableControls()
        {
            EnableDisableDateFiltering();
            EnableDisableExclusions();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void btnCancelSearch_Click(object sender, EventArgs e)
        {
            _searchCancellationTokenSource?.Cancel();
            _searchCancellationTokenSource?.Dispose();
            _searchCancellationTokenSource = null;
        }

        private void chkFilterByDate_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableDateFiltering();
            if (chkFilterByDate.Checked)
            {
                txtDateFilterAmount.Focus();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.M))
            {
                chkFilterByDate.Checked = true;
                EnableDisableDateFiltering();
                txtDateFilterAmount.Focus();
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }
    }
}
