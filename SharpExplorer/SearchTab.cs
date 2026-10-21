using SharpExplorer.Controls;
using SharpExplorer.Domain;
using SharpExplorer.Domain.FileType;
using SharpExplorer.Domain.History;
using SharpExplorer.Domain.TextSearch;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;

namespace SharpExplorer;

public partial class SearchTab : UserControl
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Index { get; init; }

    public event EventHandler<FilesFoundUpdateEventArgs>? FilesFoundUpdate;

    private readonly FileSearch _currentSearch;
    private readonly TextCriterionHistory _baseDirHistory = new(15, UserSearchRepository.BaseDirHistoryFileName);
    private readonly TextCriterionHistory _exclusionsHistory = new(15, UserSearchRepository.ExclusionsHistoryFileName);
    private readonly TextCriterionHistory _inclusionsHistory = new(15, UserSearchRepository.InclusionsHistoryFileName);
    private readonly TextCriterionHistory _searchTextHistory = new(15, UserSearchRepository.SearchTextHistoryFileName);

    private readonly TextCriterionHistory[] _histories;
    private readonly Brush[] _brushes = [.. new[] { Color.Black, Color.Orange, Color.Purple, Color.CornflowerBlue, Color.DarkKhaki, Color.Green }.Select(c => new SolidBrush(c))];

    private int contextMenuRowIndex = -1;

    private bool _anchorByHandInitialized = false;
    private bool _readyToInitializeAnchorByHand = false;
    private int _cmbExcludeFixedLeft, _cmbExcludeFixedRight, _chkEnableFixedRight;
    private int _cmbContainingTextFixedLeft, _cmbContainingTextFixedRight;
    private int _chkMatchCaseFixedRight;
    private CancellationTokenSource? _searchCancellationTokenSource;
    private bool _isClosing = false;
    private readonly UserTabSettings _userTabSettings;

    private SearchForm _parent;

    public SearchTab(UserTabState initialState)
    {
        InitializeComponent();

        _readyToInitializeAnchorByHand = true;
        AnchorByHand();

        Index = initialState.Id;
        _histories = [_baseDirHistory, _exclusionsHistory, _inclusionsHistory, _searchTextHistory];

        var resultTable = SearchResultDataTableHelper.CreateFileResultDataTableColumns();
        var searchResultDataSource = new BindingSource(resultTable.DefaultView, null!);
        _currentSearch = new(resultTable, 50);

        grdResult.AutoGenerateColumns = false;
        grdResult.DataSource = searchResultDataSource;

        _currentSearch.SearchProgressUpdate += HandleSearchStatusUpdate;

        foreach (var historyList in _histories)
        {
            historyList.Initialize();
        }

        ApplyUserSearchDto(initialState.SearchDto);
        _userTabSettings = initialState.TabSettings;
        _currentSearch.DataTable.DefaultView.Sort = _userTabSettings.ResultSortOrder;
    }

    public void InitialFocus()
    {
        cmbFilePattern.Focus();
        cmbContainingText.SelectionLength = 0;
        cmbExceptFilePattern.SelectionLength = 0;
    }

    public void ClearHistory()
    {
        foreach (var historyList in _histories)
        {
            historyList.Clear();
        }

        cmbFilePattern.Items.Clear();
        cmbExceptFilePattern.Items.Clear();
        cmbRootDir.Items.Clear();
        cmbContainingText.Items.Clear();
    }

    public UserTabState GetCurrentSettings()
    {
        var searchPatterns = cmbFilePattern.Text.Trim();
        var exceptSearchPatterns = cmbExceptFilePattern.Text.Trim();
        var directoryPath = string.IsNullOrEmpty(cmbRootDir.Text) ? "." : cmbRootDir.Text;
        var query = cmbContainingText.Text;
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

        var searchDto = new UserSearchDto
        {
            Id = Index,
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

        return new(searchDto, _userTabSettings);
    }

    private void ApplyUserSearchDto(UserSearchDto dto)
    {
        cmbFilePattern.Text = dto.RawIncludeFilesPattern ?? "";
        cmbExceptFilePattern.Text = dto.RawExcludeFilesPattern ?? "";
        cmbContainingText.Text = dto.TextSearchPattern ?? "";
        cmbContainingText.SelectionLength = 0;
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

        PopulateHistoryDropdowns(dto.EnableFileExclusion);
    }

    private FileDisplay? GetRowFileDisplay(int rowIndex)
    {
        return grdResult.GetRowFileDisplay(rowIndex);
    }

    private void HandleSearchStatusUpdate(object? sender, SearchProgressEventArgs e)
    {
        if (_isClosing)
        {
            return;
        }
        SetStatusText(e.StatusMessage);
        SetProgress(e.ProgressPercentage);
        RaiseFilesFoundUpdate(searchStatus: FileSearchStatus.Running);
    }

    private void RaiseFilesFoundUpdate(int? fileCount = null, FileSearchStatus searchStatus = FileSearchStatus.NotRunning)
    {
        if (FilesFoundUpdate != null)
        {
            var countToReport = fileCount ?? grdResult.Rows.Count;
            FilesFoundUpdate?.Invoke(this, new(countToReport, searchStatus));
        }
    }

    private void SetStatusText(string text)
    {
        lblTabStatus.Text = text;
    }

    public void SetProgress(int progressPercentage)
    {
        TabProgressBar.Visible = true;
        TabProgressBar.Value = Math.Min(Math.Max(progressPercentage, 0), 100);
    }

    public void HideProgressBar()
    {
        TabProgressBar.Visible = false;
    }

    private void ClearStatusText() => SetStatusText("");

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        SetStatusText("Searching...");
        btnSearch.Enabled = false;
        btnCancelSearch.Enabled = true;
        cmbExceptFilePattern.DroppedDown = false;
        cmbRootDir.DroppedDown = false;
        SetProgress(0);
        RaiseFilesFoundUpdate(-1, searchStatus: FileSearchStatus.Running);
        lstTextMatches.Items.Clear();
        contextMenuRowIndex = -1;
        _searchCancellationTokenSource = new CancellationTokenSource();

        var searchDto = GetCurrentSettings().SearchDto;
        spltResultPanel.FixedPanel = searchDto.HasTextSearch() ? FixedPanel.Panel1 : FixedPanel.Panel2;

        SearchOutcome? searchOutcome = null;
        try
        {
            searchOutcome = await _currentSearch.SearchAndPopulateTableAsync(searchDto, _searchCancellationTokenSource.Token);
            if (!_isClosing)
            {
                SetStatusText(searchOutcome.StatusMessage);
            }
        }
        catch
        {
            SetStatusText("Invalid search criteria");
        }

        UserSearchRepository.SaveSearchDto(searchDto);
        _baseDirHistory.AddItem(searchDto.BaseDirectoriesPattern);
        _inclusionsHistory.AddItem(searchDto.RawIncludeFilesPattern);
        _searchTextHistory.AddItem(searchDto.TextSearchPattern);
        if (searchDto.EnableFileExclusion)
        {
            _exclusionsHistory.AddItem(searchDto.RawExcludeFilesPattern);
        }

        PopulateHistoryDropdowns(searchDto.EnableFileExclusion);

        grdResult.FocusAfterSearch();

        btnSearch.Enabled = true;
        btnCancelSearch.Enabled = false;
        HideProgressBar();
        RaiseFilesFoundUpdate(searchStatus: searchOutcome?.FinalState ?? FileSearchStatus.Failed);
    }

    private void PopulateHistoryDropdowns(bool updateExclusions)
    {
        cmbRootDir.ApplyComboBoxHistory(_baseDirHistory);
        cmbFilePattern.ApplyComboBoxHistory(_inclusionsHistory);
        cmbContainingText.ApplyComboBoxHistory(_searchTextHistory);
        cmbExceptFilePattern.ApplyComboBoxHistory(_exclusionsHistory);
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

    private static bool ExploreFile(string filePath)
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
            cmbRootDir.FocusWithEndCursor();
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

    private void mnuExcludeThisFolder_Click(object sender, EventArgs e)
    {
        var selectedFileDisplay = GetRowFileDisplay(contextMenuRowIndex);
        if (selectedFileDisplay == null)
            return;

        var folderToExclude = selectedFileDisplay.Location.Location;
        var exclusionsPlusNew = _currentSearch.SearchCriteria?.AddFolderExclusion(folderToExclude);
        if (exclusionsPlusNew != null)
        {
            cmbExceptFilePattern.Text = exclusionsPlusNew;
            chkEnableExclusions.Checked = true;
        }
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
            xPos += g.MeasureString(segmentText, segmentFont, 0, MeasuringHelper.FormatForMeasuring).Width;
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

    private void SearchTab_Load(object sender, EventArgs e)
    {
        _parent = (this.Parent!.Parent!.Parent as SearchForm)!;

        ColumnHeader header = new()
        {
            Text = "",
            Name = "col1",
            Width = 2000
        };
        lstTextMatches.Columns.Add(header);

        SetInputFont(this);

        EnableDisableControls();

        grdResult.Initialize();

        ResizeResultsGrid();

        cmbFilePattern.Select();
    }

    private void SetInputFont(Control parentControl)
    {
        var inputFontFamilyName = "Cascadia Mono";
        foreach (var control in parentControl.Controls)
        {
            switch (control)
            {
                case TextBox textBox:
                    textBox.AutoSize = false;
                    textBox.Font = new Font(inputFontFamilyName, this.Font.Size, this.Font.Style);
                    break;
                case ComboBox comboBox:
                    comboBox.AutoSize = false;
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

        _readyToInitializeAnchorByHand = true;
    }

    private void grdResult_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
    {
        contextMenuRowIndex = e.RowIndex;

        var enableOpenWithTextEditor = false;

        if (_currentSearch?.SearchCriteria?.HasTextSearch() ?? false)
        {
            enableOpenWithTextEditor = true; // We found the text in it, so we assume it's a text file.
        }
        else
        {
            var selectedFileDisplay = GetRowFileDisplay(contextMenuRowIndex);
            if (FileTypeHelper.IsKnownTextFile(selectedFileDisplay?.GetFileExtension()))
            {
                // They didn't do a text search,  but the file extension is known to be a text file.
                enableOpenWithTextEditor = true;
            }
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
        cmbExceptFilePattern.SelectAll();
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
        cmbExceptFilePattern.Enabled = chkEnableExclusions.Checked;
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
            cmbExceptFilePattern.Enabled = true;
            chkEnableExclusions.Checked = true;
            cmbExceptFilePattern.Focus();
            return true;
        }
        else
        {
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    public void CancelSearch()
    {
        _isClosing = true;
        _searchCancellationTokenSource?.Cancel();
        _searchCancellationTokenSource?.Dispose();
        _searchCancellationTokenSource = null;
        ClearStatusText();
        HideProgressBar();
    }

    private void cmbRootDir_TextUpdate(object sender, EventArgs e)
    {
        var currPath = cmbRootDir.Text;

        var possibleContinuations = DirectoryPredictiveTexter.GetPossibleContinuations(currPath);
        cmbRootDir.PopulateDropdown(_baseDirHistory, possibleContinuations);
    }

    private void cmbRootDir_Enter(object sender, EventArgs e)
    {
        if (sender is PredictiveComboBox cmbRootDir)
        {
            cmbRootDir.SelectAll();
            cmbRootDir.ApplyComboBoxHistory(_baseDirHistory);
        }
    }

    private void MoveComboBoxCaretToEnd(PredictiveComboBox comboBox)
    {
        BeginInvoke((MethodInvoker)(() =>
        {
            comboBox.MoveCursorToEnd();
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
        // Our tooltips are *mouse only*.

        if (e.ColumnIndex == grdResult.LocationColumnIndex)
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
        if (grdResult.LocationColumnIndex < 0)
        {
            return;
        }

        var newWidth = spltResultPanel.Panel1.Width - (SystemInformation.VerticalScrollBarWidth + 5);
        grdResult.ResizeResultsGrid(newWidth);
        //var fixedColWidths = 0;
        //for (int i = 0; i < grdResult.Columns.Count; i++)
        //{
        //    if (i != _locationColIndex)
        //    {
        //        fixedColWidths += grdResult.Columns[i].Width;
        //    }
        //}

        //if (fixedColWidths == 0)
        //{
        //    return;
        //}

        //grdResult.Columns[_locationColIndex].Width = Math.Max(50, newWidth - fixedColWidths);
    }

    private void spltResultPanel_SizeChanged(object sender, EventArgs e)
    {
    }

    private void grdResult_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
    {
        if (e.Column.Index == grdResult.LocationColumnIndex)
        {
            RefreshLocationDisplayStringsAfterColumnResize();
        }
    }

    private void RefreshLocationDisplayStringsAfterColumnResize()
    {
        var maxLocationLength = grdResult.GetCurrentLocationColumnCapacity();
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

    private void grdResult_MouseEnter(object sender, EventArgs e)
    {
        grdResult.ShowCellToolTips = true;
    }

    private void grdResult_MouseLeave(object sender, EventArgs e)
    {
        grdResult.ShowCellToolTips = false;
    }

    private void AnchorByHand()
    {
        if (!_readyToInitializeAnchorByHand)
        {
            return;
        }

        // HACK: this is my workaround for the bug where right-anchored combobox is not initially drawn.
        EnsureAnchorByHandInitialized();

        cmbExceptFilePattern.Width = this.Width - (_cmbExcludeFixedLeft + _cmbExcludeFixedRight);
        cmbExceptFilePattern.Left = _cmbExcludeFixedLeft;
        chkEnableExclusions.Left = this.Width - (_chkEnableFixedRight + chkEnableExclusions.Width);
        cmbContainingText.Width = this.Width - (_cmbContainingTextFixedLeft + _cmbContainingTextFixedRight);
        cmbContainingText.Left = _cmbContainingTextFixedLeft;
        chkMatchCase.Left = this.Width - (_chkMatchCaseFixedRight + chkMatchCase.Width);
        chkMatchWholeWord.Left = chkMatchCase.Left;
    }

    private void EnsureAnchorByHandInitialized()
    {
        if (_readyToInitializeAnchorByHand
            && !_anchorByHandInitialized)
        {
            var thisWidth = this.Width;
            _cmbExcludeFixedLeft = cmbExceptFilePattern.Left;
            _cmbExcludeFixedRight = thisWidth - (cmbExceptFilePattern.Right);
            _chkEnableFixedRight = thisWidth - (chkEnableExclusions.Right);
            _cmbContainingTextFixedLeft = cmbContainingText.Left;
            _cmbContainingTextFixedRight = thisWidth - (cmbContainingText.Right);
            _chkMatchCaseFixedRight = thisWidth - (chkMatchCase.Right);
            _anchorByHandInitialized = true;
        }
    }

    private void SearchTab_Resize(object sender, EventArgs e)
    {
        AnchorByHand();
    }

    private void grdResult_Sorted(object sender, EventArgs e)
    {
        _userTabSettings.ResultSortOrder = _currentSearch.DataTable.DefaultView.Sort;
    }
}