using SharpExplorer.Domain;
using SharpExplorer.Domain.FileType;
using SharpExplorer.Domain.TextSearch;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;

namespace SharpExplorer;

public partial class SearchTab : UserControl
{
    private readonly FileSearch _currentSearch;
    private readonly TextCriterionHistory _baseDirHistory = new(15);

    private readonly Brush[] _brushes = [.. new[] { Color.Black, Color.Orange, Color.Purple, Color.CornflowerBlue, Color.DarkKhaki, Color.Green }.Select(c => new SolidBrush(c))];
    private readonly StringFormat _formatForMeasuring;

    private int contextMenuRowIndex = -1;
    private int _locationColIndex = -1;
    private CancellationTokenSource? _searchCancellationTokenSource;
    private bool _isClosing = false;

    public SearchTab()
    {
        InitializeComponent();

        var resultTable = SearchResultDataTableHelper.CreateFileResultDataTableColumns();
        var searchResultDataSource = new BindingSource(resultTable.DefaultView, null!);
        _currentSearch = new(resultTable, 50);

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
        var searchPatterns = txtFilePattern.Text.Trim();
        var exceptSearchPatterns = txtExceptFilePattern.Text.Trim();
        var directoryPath = string.IsNullOrEmpty(cmbRootDir.Text) ? "." : cmbRootDir.Text;
        var query = txtContainingText.Text;
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
            BaseDirectoriesPattern = UserSearchDto.NormalizeDirectoryName(directoryPath),
            EnableFileExclusion = chkEnableExclusions.Checked,
            FilterByModifiedDate = chkFilterByDate.Checked,
            ShowHiddenItems = chkShowHidden.Checked,
            ModifiedInLastQuantity = userSearchDateFilterQty,
            ModifiedInLastQuantityType = cmbDateFilterAmountType.SelectedItem?.ToString(),
        };
    }

    private void ApplyUserSearchDto(UserSearchDto dto)
    {
        txtFilePattern.Text = dto.RawIncludeFilesPattern ?? "";
        txtExceptFilePattern.Text = dto.RawExcludeFilesPattern ?? "";
        txtContainingText.Text = dto.TextSearchPattern ?? "";
        cmbRootDir.Text = dto.BaseDirectoriesPattern ?? "";
        chkEnableExclusions.Checked = dto.EnableFileExclusion;
        chkShowHidden.Checked = dto.ShowHiddenItems;
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
        if (_isClosing)
        {
            return;
        }
        SetStatusText(e.StatusMessage);
        searchProgressBar.Value = Math.Min(Math.Max(e.ProgressPercentage, 0), 100);
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
        cmbRootDir.DroppedDown = false;
        searchProgressBar.Visible = true;
        lstTextMatches.Items.Clear();
        contextMenuRowIndex = -1;
        _searchCancellationTokenSource = new CancellationTokenSource();

        var searchDto = CreateUserSearchDto();
        spltResultPanel.FixedPanel = searchDto.HasTextSearch() ? FixedPanel.Panel1 : FixedPanel.Panel2;

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
        _baseDirHistory.AddItem(searchDto.BaseDirectoriesPattern);
        ApplyComboBoxHistory(cmbRootDir, _baseDirHistory);

        var numMatches = grdResult.Rows.Count;
        if (numMatches > 0)
        {
            grdResult.Rows[0].Selected = true;
            grdResult.CurrentCell = grdResult.Rows[0].Cells[0];
            grdResult.Focus();
        }

        btnSearch.Enabled = true;
        btnCancelSearch.Enabled = false;
        searchProgressBar.Visible = false;
    }

    private void mnuCopyPath_Click(object sender, EventArgs e)
    {
        var selectedItems = grdResult.SelectedRows;
        if (selectedItems.Count == 0)
        {
            return;
        }
        List<string> filesToCopy = [];
        List<int> selectedRowIndexes = [];
        foreach (var selectedRow in selectedItems)
        {
            if (selectedRow is DataGridViewRow row)
            {
                selectedRowIndexes.Add(row.Index);
            }
        }
        selectedRowIndexes.Sort();
        foreach (var rowIndex in selectedRowIndexes)
        {
            var selectedFile = GetRowFileDisplay(rowIndex);
            if (selectedFile != null)
            {
                filesToCopy.Add(selectedFile.FullPath);
            }
        }

        var pathsText = string.Join(Environment.NewLine, filesToCopy.Select(GetSafeFileName));
        Clipboard.SetText(pathsText);
    }


    private void mnuCopyFolderPath_Click(object sender, EventArgs e)
    {
        var selectedItems = grdResult.SelectedRows;
        if (selectedItems.Count == 0)
        {
            return;
        }
        List<string> filesToCopy = [];
        List<int> selectedRowIndexes = [];
        foreach (var selectedRow in selectedItems)
        {
            if (selectedRow is DataGridViewRow row)
            {
                selectedRowIndexes.Add(row.Index);
            }
        }
        selectedRowIndexes.Sort();
        foreach (var rowIndex in selectedRowIndexes)
        {
            var selectedFile = GetRowFileDisplay(rowIndex);
            if (selectedFile != null)
            {
                filesToCopy.Add(selectedFile.Location.Location);
            }
        }

        var pathsText = string.Join(Environment.NewLine, filesToCopy.Select(GetSafeFileName).Distinct());
        Clipboard.SetText(pathsText);
    }

    private static string GetSafeFileName(string filePath) => filePath.Contains(' ') ? $"\"{filePath}\"" : filePath;

    private void mnuCopyFiles_Click(object sender, EventArgs e)
    {
        var selectedItems = grdResult.SelectedRows;
        StringCollection filesToCopy = [];
        foreach (var selectedRow in selectedItems)
        {
            if (selectedRow is DataGridViewRow row)
            {
                var selectedFile = GetRowFileDisplay(row.Index);
                if (selectedFile != null)
                {
                    filesToCopy.Add(selectedFile.FullPath);
                }
            }
        }

        if (filesToCopy.Count > 0)
        {
            Clipboard.SetFileDropList(filesToCopy);
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

        ExploreFile(selectedFilePath);
    }

    public bool ExploreFile(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
        {
            return false;
        }
        // Clean up file path so it can be navigated OK
        filePath = System.IO.Path.GetFullPath(filePath);
        System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", filePath));
        return true;
    }

    private void mnuSearchHere_Click(object sender, EventArgs e)
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
            cmbRootDir.Text = containingFolder;
            cmbRootDir.Focus();
            cmbRootDir.SelectionStart = cmbRootDir.Text.Length;
            cmbRootDir.SelectionLength = 0;
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

    private float _gridTextBenchmarkWidth = 0; // The width of a benchmark 10-character string.

    private int GetGridCellCapacity(int cellWidth)
    {
        // Return the number of characters that can fit in a cell of the given width, based on the benchmark width.
        if (_gridTextBenchmarkWidth == 0)
        {
            using Graphics g = grdResult.CreateGraphics();
            _gridTextBenchmarkWidth = g.MeasureString(@"C:\Wapita\", grdResult.Font, 0, _formatForMeasuring).Width;
        }

        return (int)(cellWidth / _gridTextBenchmarkWidth * 10) - 4; // Allow for some padding and ellipsis.
    }

    private int GetCurrentGridCellCapacity(int colIndex)
    {
        return GetGridCellCapacity(grdResult.Columns[colIndex].Width);
    }

    private int GetCurrentLocationColumnCapacity()
    {
        return GetCurrentGridCellCapacity(_locationColIndex);
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

        SetInputFont(this);

        EnableDisableControls();

        _locationColIndex = grdResult.Columns["colLocation"].Index!;

        ResizeResultsGrid();
    }

    private void SetInputFont(Control parentControl)
    {
        var inputFontFamilyName = "Cascadia Mono";
        foreach (var control in parentControl.Controls)
        {
            switch (control)
            {
                case TextBox textBox:
                    textBox.Font = new Font(inputFontFamilyName, this.Font.Size, this.Font.Style);
                    break;
                case ComboBox comboBox:
                    comboBox.Font = new Font(inputFontFamilyName, this.Font.Size, this.Font.Style);
                    break;
                case ListView listView when listView.Name == nameof(lstTextMatches):
                    listView.Font = new Font(inputFontFamilyName, this.Font.Size, this.Font.Style);
                    break;
            }

            if (control is Control childControl)
            {
                SetInputFont(childControl);
            }
        }
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
        else if (keyData == (Keys.Alt | Keys.X))
        {
            txtExceptFilePattern.Enabled = true;
            chkEnableExclusions.Checked = true;
            txtExceptFilePattern.Focus();
            return true;
        }
        else
        {
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    //protected override void OnFormClosing(FormClosingEventArgs e)
    //{
    //    _isClosing = true;
    //    base.OnFormClosing(e);
    //}

    private void cmbRootDir_TextUpdate(object sender, EventArgs e)
    {
        var caretPos = cmbRootDir.SelectionStart;
        var currPath = cmbRootDir.Text;
        var possibleContinuations = DirectoryPredictiveTexter.GetPossibleContinuations(currPath);

        if (!ApplyComboBoxHistory(cmbRootDir, _baseDirHistory))
        {
            cmbRootDir.Items.Clear();
            cmbRootDir.Items.Add(currPath);
            cmbRootDir.Items.AddRange(possibleContinuations);
        }

        if (cmbRootDir.Items.Count > 0)
        {
            cmbRootDir.DroppedDown = true;
        }

        cmbRootDir.SelectionStart = Math.Min(caretPos, cmbRootDir.Text.Length);
    }

    private void cmbRootDir_Enter(object sender, EventArgs e)
    {
        if (sender is ComboBox cmbRootDir)
        {
            cmbRootDir.SelectAll();
            ApplyComboBoxHistory(cmbRootDir, _baseDirHistory);
        }
    }

    private static bool ApplyComboBoxHistory(ComboBox comboBox, TextCriterionHistory history)
    {
        if (comboBox.Text != "")
        {
            return false;
        }
        comboBox.Items.Clear();
        foreach (var item in history.Items)
        {
            comboBox.Items.Add(item);
        }
        return true;
    }

    private void MoveComboBoxCaretToEnd(ComboBox comboBox)
    {
        BeginInvoke((MethodInvoker)(() =>
        {
            comboBox.SelectionStart = comboBox.Text.Length;
            comboBox.SelectionLength = 0; // remove selection highlight
        }));
    }

    private void cmbRootDir_SelectedIndexChanged(object sender, EventArgs e)
    {
        MoveComboBoxCaretToEnd(cmbRootDir);
    }

    private void cmbRootDir_SelectedValueChanged(object sender, EventArgs e)
    {
        MoveComboBoxCaretToEnd(cmbRootDir);
    }

    private void grdResult_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
    {
        if (e.ColumnIndex == _locationColIndex)
        {
            var fileDisplay = GetRowFileDisplay(e.RowIndex);
            if (fileDisplay != null)
            {
                e.ToolTipText = fileDisplay.LocationTooltip;
            }
        }
        else
        {
            e.ToolTipText = null;
        }
    }

    private void lstTextMatches_SelectedIndexChanged(object sender, EventArgs e)
    {
    }


    private void spltResultPanel_Panel1_Resize(object sender, EventArgs e)
    {
        ResizeResultsGrid();
    }

    private void ResizeResultsGrid()
    {
        if (_locationColIndex < 0)
        {
            return;
        }

        var newWidth = spltResultPanel.Panel1.Width - (SystemInformation.VerticalScrollBarWidth + 5);
        var fixedColWidths = 0;
        for (int i = 0; i < grdResult.Columns.Count; i++)
        {
            if (i != _locationColIndex)
            {
                fixedColWidths += grdResult.Columns[i].Width;
            }
        }

        if (fixedColWidths == 0)
        {
            return;
        }

        grdResult.Columns[_locationColIndex].Width = Math.Max(50, newWidth - fixedColWidths);
    }

    private void spltResultPanel_SizeChanged(object sender, EventArgs e)
    {
    }

    private void grdResult_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
    {
        if (e.Column.Index == _locationColIndex)
        {
            RefreshLocationDisplayStringsAfterColumnResize();
        }
    }

    private void RefreshLocationDisplayStringsAfterColumnResize()
    {
        var maxLocationLength = GetCurrentLocationColumnCapacity();
        _currentSearch?.SetLocationDisplayLength(maxLocationLength);
    }

    private void mnuTextMatchLineCopy_Click(object sender, EventArgs e)
    {
        var selectedTextMatchItem = lstTextMatches.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
        if (selectedTextMatchItem == null)
        {
            return;
        }

        if (selectedTextMatchItem.Tag is FileLineTextMatch match)
        {
            Clipboard.SetText(match.Line);
        }
    }
}