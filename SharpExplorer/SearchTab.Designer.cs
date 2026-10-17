using SharpExplorer.Controls;

namespace SharpExplorer
{
    partial class SearchTab
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            btnSearch = new Button();
            cmbFilePattern = new ComboBox();
            txtContainingText = new TextBox();
            mnuFileItemContext = new ContextMenuStrip(components);
            mnuOpenFile = new ToolStripMenuItem();
            mnuOpenWithTextEditor = new ToolStripMenuItem();
            mnuShowInExplorer = new ToolStripMenuItem();
            mnuCopyPath = new ToolStripMenuItem();
            mnuCopyFolderPath = new ToolStripMenuItem();
            mnuCopyFiles = new ToolStripMenuItem();
            mnuSearchHere = new ToolStripMenuItem();
            mnuExcludeThisFolder = new ToolStripMenuItem();
            chkMatchCase = new CheckBox();
            chkMatchWholeWord = new CheckBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            spltResultPanel = new SplitContainer();
            grdResult = new SearchResultDataGridView();
            colIcon = new DataGridViewImageColumn();
            colName = new DataGridViewTextBoxColumn();
            colLocation = new DataGridViewTextBoxColumn();
            colFileType = new DataGridViewTextBoxColumn();
            colSize = new DataGridViewTextBoxColumn();
            colLastModified = new DataGridViewTextBoxColumn();
            lstTextMatches = new ListView();
            mnuTextMatchContext = new ContextMenuStrip(components);
            mnuTextMatchLineCopy = new ToolStripMenuItem();
            label4 = new Label();
            cmbExceptFilePattern = new ComboBox();
            chkEnableExclusions = new CheckBox();
            btnCancelSearch = new Button();
            chkFilterByDate = new CheckBox();
            txtDateFilterAmount = new TextBox();
            cmbDateFilterAmountType = new ComboBox();
            cmbRootDir = new PredictiveComboBox();
            chkShowHidden = new CheckBox();
            mnuFileItemContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)spltResultPanel).BeginInit();
            spltResultPanel.Panel1.SuspendLayout();
            spltResultPanel.Panel2.SuspendLayout();
            spltResultPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdResult).BeginInit();
            mnuTextMatchContext.SuspendLayout();
            SuspendLayout();
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(23, 190);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(94, 29);
            btnSearch.TabIndex = 14;
            btnSearch.Text = "&Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // cmbFilePattern
            // 
            cmbFilePattern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbFilePattern.Location = new Point(123, 6);
            cmbFilePattern.Name = "cmbFilePattern";
            cmbFilePattern.Size = new Size(872, 28);
            cmbFilePattern.TabIndex = 1;
            cmbFilePattern.Text = "*.txt";
            // 
            // txtContainingText
            // 
            txtContainingText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtContainingText.Location = new Point(123, 96);
            txtContainingText.Name = "txtContainingText";
            txtContainingText.Size = new Size(729, 27);
            txtContainingText.TabIndex = 5;
            txtContainingText.Enter += txtContainingText_Enter;
            // 
            // mnuFileItemContext
            // 
            mnuFileItemContext.ImageScalingSize = new Size(20, 20);
            mnuFileItemContext.Items.AddRange(new ToolStripItem[] { mnuOpenFile, mnuOpenWithTextEditor, mnuShowInExplorer, mnuCopyPath, mnuCopyFolderPath, mnuCopyFiles, mnuSearchHere, mnuExcludeThisFolder });
            mnuFileItemContext.Name = "mnuFileItemContext";
            mnuFileItemContext.Size = new Size(225, 196);
            // 
            // mnuOpenFile
            // 
            mnuOpenFile.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            mnuOpenFile.Name = "mnuOpenFile";
            mnuOpenFile.Size = new Size(224, 24);
            mnuOpenFile.Text = "Open";
            mnuOpenFile.Click += mnuOpenFile_Click;
            // 
            // mnuOpenWithTextEditor
            // 
            mnuOpenWithTextEditor.Name = "mnuOpenWithTextEditor";
            mnuOpenWithTextEditor.Size = new Size(224, 24);
            mnuOpenWithTextEditor.Text = "Open With Text Editor";
            mnuOpenWithTextEditor.Click += mnuOpenWithTextEditor_Click;
            // 
            // mnuShowInExplorer
            // 
            mnuShowInExplorer.Name = "mnuShowInExplorer";
            mnuShowInExplorer.Size = new Size(224, 24);
            mnuShowInExplorer.Text = "Open In File Explorer";
            mnuShowInExplorer.Click += mnuShowInExplorer_Click;
            // 
            // mnuCopyPath
            // 
            mnuCopyPath.Name = "mnuCopyPath";
            mnuCopyPath.ShortcutKeys = Keys.Control | Keys.C;
            mnuCopyPath.Size = new Size(224, 24);
            mnuCopyPath.Text = "Copy Full Path";
            mnuCopyPath.Click += mnuCopyPath_Click;
            // 
            // mnuCopyFolderPath
            // 
            mnuCopyFolderPath.Name = "mnuCopyFolderPath";
            mnuCopyFolderPath.Size = new Size(224, 24);
            mnuCopyFolderPath.Text = "Copy Folder Path";
            mnuCopyFolderPath.Click += mnuCopyFolderPath_Click;
            // 
            // mnuCopyFiles
            // 
            mnuCopyFiles.Name = "mnuCopyFiles";
            mnuCopyFiles.Size = new Size(224, 24);
            mnuCopyFiles.Text = "Copy";
            mnuCopyFiles.Click += mnuCopyFiles_Click;
            // 
            // mnuSearchHere
            // 
            mnuSearchHere.Name = "mnuSearchHere";
            mnuSearchHere.Size = new Size(224, 24);
            mnuSearchHere.Text = "Search This Folder...";
            mnuSearchHere.Click += mnuSearchHere_Click;
            // 
            // mnuExcludeThisFolder
            // 
            mnuExcludeThisFolder.Name = "mnuExcludeThisFolder";
            mnuExcludeThisFolder.Size = new Size(224, 24);
            mnuExcludeThisFolder.Text = "Exclude This Folder...";
            mnuExcludeThisFolder.Click += mnuExcludeThisFolder_Click;
            // 
            // chkMatchCase
            // 
            chkMatchCase.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkMatchCase.AutoSize = true;
            chkMatchCase.Location = new Point(864, 86);
            chkMatchCase.Name = "chkMatchCase";
            chkMatchCase.Size = new Size(107, 24);
            chkMatchCase.TabIndex = 11;
            chkMatchCase.Text = "Match &Case";
            chkMatchCase.UseVisualStyleBackColor = true;
            // 
            // chkMatchWholeWord
            // 
            chkMatchWholeWord.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkMatchWholeWord.AutoSize = true;
            chkMatchWholeWord.Location = new Point(864, 112);
            chkMatchWholeWord.Name = "chkMatchWholeWord";
            chkMatchWholeWord.Size = new Size(114, 24);
            chkMatchWholeWord.TabIndex = 12;
            chkMatchWholeWord.Text = "Whole &Word";
            chkMatchWholeWord.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 8);
            label1.Name = "label1";
            label1.Size = new Size(76, 20);
            label1.TabIndex = 0;
            label1.Text = "&File name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 99);
            label2.Name = "label2";
            label2.Size = new Size(113, 20);
            label2.TabIndex = 4;
            label2.Text = "Containing &text:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 159);
            label3.Name = "label3";
            label3.Size = new Size(60, 20);
            label3.TabIndex = 6;
            label3.Text = "&Look in:";
            // 
            // spltResultPanel
            // 
            spltResultPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            spltResultPanel.FixedPanel = FixedPanel.Panel1;
            spltResultPanel.Location = new Point(5, 224);
            spltResultPanel.Name = "spltResultPanel";
            // 
            // spltResultPanel.Panel1
            // 
            spltResultPanel.Panel1.Controls.Add(grdResult);
            spltResultPanel.Panel1.Resize += spltResultPanel_Panel1_Resize;
            // 
            // spltResultPanel.Panel2
            // 
            spltResultPanel.Panel2.Controls.Add(lstTextMatches);
            spltResultPanel.Size = new Size(996, 290);
            spltResultPanel.SplitterDistance = 710;
            spltResultPanel.SplitterWidth = 5;
            spltResultPanel.TabIndex = 101;
            // 
            // grdResult
            // 
            grdResult.AllowUserToAddRows = false;
            grdResult.AllowUserToDeleteRows = false;
            grdResult.AllowUserToResizeRows = false;
            grdResult.CellBorderStyle = DataGridViewCellBorderStyle.None;
            grdResult.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdResult.Columns.AddRange(new DataGridViewColumn[] { colIcon, colName, colLocation, colFileType, colSize, colLastModified });
            grdResult.ContextMenuStrip = mnuFileItemContext;
            grdResult.Dock = DockStyle.Fill;
            grdResult.Location = new Point(0, 0);
            grdResult.Name = "grdResult";
            grdResult.ReadOnly = true;
            grdResult.RowHeadersVisible = false;
            grdResult.RowHeadersWidth = 51;
            grdResult.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdResult.ShowCellToolTips = false;
            grdResult.Size = new Size(710, 290);
            grdResult.TabIndex = 100;
            grdResult.CellContextMenuStripNeeded += grdResult_CellContextMenuStripNeeded;
            grdResult.CellMouseDoubleClick += grdResult_CellMouseDoubleClick;
            grdResult.CellToolTipTextNeeded += grdResult_CellToolTipTextNeeded;
            grdResult.ColumnWidthChanged += grdResult_ColumnWidthChanged;
            grdResult.RowEnter += grdResult_RowEnter;
            grdResult.MouseEnter += grdResult_MouseEnter;
            grdResult.MouseLeave += grdResult_MouseLeave;
            // 
            // colIcon
            // 
            colIcon.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colIcon.DataPropertyName = "Icon";
            colIcon.HeaderText = "";
            colIcon.MinimumWidth = 6;
            colIcon.Name = "colIcon";
            colIcon.ReadOnly = true;
            colIcon.Width = 22;
            // 
            // colName
            // 
            colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colName.DataPropertyName = "Name";
            colName.HeaderText = "Name";
            colName.MinimumWidth = 6;
            colName.Name = "colName";
            colName.ReadOnly = true;
            colName.Width = 200;
            // 
            // colLocation
            // 
            colLocation.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colLocation.DataPropertyName = "Location";
            colLocation.HeaderText = "Location";
            colLocation.MinimumWidth = 6;
            colLocation.Name = "colLocation";
            colLocation.ReadOnly = true;
            colLocation.Width = 95;
            // 
            // colFileType
            // 
            colFileType.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colFileType.DataPropertyName = "FileTypeDescription";
            colFileType.HeaderText = "Type";
            colFileType.MinimumWidth = 6;
            colFileType.Name = "colFileType";
            colFileType.ReadOnly = true;
            colFileType.Width = 69;
            // 
            // colSize
            // 
            colSize.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colSize.DataPropertyName = "SizeDisplay";
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleRight;
            colSize.DefaultCellStyle = dataGridViewCellStyle2;
            colSize.HeaderText = "Size";
            colSize.MinimumWidth = 6;
            colSize.Name = "colSize";
            colSize.ReadOnly = true;
            colSize.Width = 65;
            // 
            // colLastModified
            // 
            colLastModified.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colLastModified.DataPropertyName = "LastModified";
            colLastModified.HeaderText = "Modified";
            colLastModified.MinimumWidth = 6;
            colLastModified.Name = "colLastModified";
            colLastModified.ReadOnly = true;
            colLastModified.Width = 99;
            // 
            // lstTextMatches
            // 
            lstTextMatches.ContextMenuStrip = mnuTextMatchContext;
            lstTextMatches.Dock = DockStyle.Fill;
            lstTextMatches.HeaderStyle = ColumnHeaderStyle.None;
            lstTextMatches.Location = new Point(0, 0);
            lstTextMatches.Name = "lstTextMatches";
            lstTextMatches.OwnerDraw = true;
            lstTextMatches.Size = new Size(281, 290);
            lstTextMatches.TabIndex = 102;
            lstTextMatches.UseCompatibleStateImageBehavior = false;
            lstTextMatches.View = View.Details;
            lstTextMatches.DrawItem += lstTextMatches_DrawItem;
            // 
            // mnuTextMatchContext
            // 
            mnuTextMatchContext.ImageScalingSize = new Size(20, 20);
            mnuTextMatchContext.Items.AddRange(new ToolStripItem[] { mnuTextMatchLineCopy });
            mnuTextMatchContext.Name = "mnuTextMatchContext";
            mnuTextMatchContext.Size = new Size(113, 28);
            // 
            // mnuTextMatchLineCopy
            // 
            mnuTextMatchLineCopy.Name = "mnuTextMatchLineCopy";
            mnuTextMatchLineCopy.Size = new Size(112, 24);
            mnuTextMatchLineCopy.Text = "Copy";
            mnuTextMatchLineCopy.Click += mnuTextMatchLineCopy_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 51);
            label4.Name = "label4";
            label4.Size = new Size(56, 20);
            label4.TabIndex = 2;
            label4.Text = "E&xcept:";
            // 
            // cmbExceptFilePattern
            // 
            cmbExceptFilePattern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbExceptFilePattern.FormattingEnabled = true;
            cmbExceptFilePattern.Location = new Point(123, 48);
            cmbExceptFilePattern.Name = "cmbExceptFilePattern";
            cmbExceptFilePattern.Size = new Size(772, 28);
            cmbExceptFilePattern.TabIndex = 3;
            cmbExceptFilePattern.Enter += txtExceptFilePattern_Enter;
            // 
            // chkEnableExclusions
            // 
            chkEnableExclusions.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkEnableExclusions.AutoSize = true;
            chkEnableExclusions.Location = new Point(901, 50);
            chkEnableExclusions.Name = "chkEnableExclusions";
            chkEnableExclusions.Size = new Size(76, 24);
            chkEnableExclusions.TabIndex = 13;
            chkEnableExclusions.Text = "E&nable";
            chkEnableExclusions.UseVisualStyleBackColor = true;
            chkEnableExclusions.CheckedChanged += chkEnableExclusions_CheckedChanged;
            // 
            // btnCancelSearch
            // 
            btnCancelSearch.Enabled = false;
            btnCancelSearch.Location = new Point(123, 190);
            btnCancelSearch.Name = "btnCancelSearch";
            btnCancelSearch.Size = new Size(94, 29);
            btnCancelSearch.TabIndex = 15;
            btnCancelSearch.Text = "Sto&p";
            btnCancelSearch.UseVisualStyleBackColor = true;
            btnCancelSearch.Click += btnCancelSearch_Click;
            // 
            // chkFilterByDate
            // 
            chkFilterByDate.AutoSize = true;
            chkFilterByDate.Location = new Point(429, 194);
            chkFilterByDate.Name = "chkFilterByDate";
            chkFilterByDate.Size = new Size(160, 24);
            chkFilterByDate.TabIndex = 8;
            chkFilterByDate.Text = "&Modified in the last";
            chkFilterByDate.UseVisualStyleBackColor = true;
            chkFilterByDate.CheckedChanged += chkFilterByDate_CheckedChanged;
            // 
            // txtDateFilterAmount
            // 
            txtDateFilterAmount.Location = new Point(585, 192);
            txtDateFilterAmount.Name = "txtDateFilterAmount";
            txtDateFilterAmount.Size = new Size(43, 27);
            txtDateFilterAmount.TabIndex = 9;
            txtDateFilterAmount.TextAlign = HorizontalAlignment.Right;
            txtDateFilterAmount.Enter += txtDateFilterAmount_Enter;
            // 
            // cmbDateFilterAmountType
            // 
            cmbDateFilterAmountType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDateFilterAmountType.FormattingEnabled = true;
            cmbDateFilterAmountType.Items.AddRange(new object[] { "hours", "days", "weeks", "months", "years" });
            cmbDateFilterAmountType.Location = new Point(634, 190);
            cmbDateFilterAmountType.Name = "cmbDateFilterAmountType";
            cmbDateFilterAmountType.Size = new Size(85, 28);
            cmbDateFilterAmountType.TabIndex = 10;
            // 
            // cmbRootDir
            // 
            cmbRootDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbRootDir.FormattingEnabled = true;
            cmbRootDir.Location = new Point(77, 156);
            cmbRootDir.Name = "cmbRootDir";
            cmbRootDir.Size = new Size(919, 28);
            cmbRootDir.TabIndex = 7;
            cmbRootDir.SelectedIndexChanged += cmbRootDir_SelectedIndexChanged;
            cmbRootDir.TextUpdate += cmbRootDir_TextUpdate;
            cmbRootDir.SelectedValueChanged += cmbRootDir_SelectedValueChanged;
            cmbRootDir.Enter += cmbRootDir_Enter;
            // 
            // chkShowHidden
            // 
            chkShowHidden.AutoSize = true;
            chkShowHidden.Location = new Point(843, 196);
            chkShowHidden.Margin = new Padding(3, 4, 3, 4);
            chkShowHidden.Name = "chkShowHidden";
            chkShowHidden.Size = new Size(120, 24);
            chkShowHidden.TabIndex = 104;
            chkShowHidden.Text = "Hidden Items";
            chkShowHidden.UseVisualStyleBackColor = true;
            // 
            // SearchTab
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(chkShowHidden);
            Controls.Add(cmbRootDir);
            Controls.Add(cmbExceptFilePattern);
            Controls.Add(cmbDateFilterAmountType);
            Controls.Add(txtDateFilterAmount);
            Controls.Add(chkFilterByDate);
            Controls.Add(btnCancelSearch);
            Controls.Add(chkEnableExclusions);
            Controls.Add(label4);
            Controls.Add(spltResultPanel);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(chkMatchWholeWord);
            Controls.Add(chkMatchCase);
            Controls.Add(txtContainingText);
            Controls.Add(cmbFilePattern);
            Controls.Add(btnSearch);
            Name = "SearchTab";
            Size = new Size(1008, 517);
            Load += SearchTab_Load;
            mnuFileItemContext.ResumeLayout(false);
            spltResultPanel.Panel1.ResumeLayout(false);
            spltResultPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)spltResultPanel).EndInit();
            spltResultPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdResult).EndInit();
            mnuTextMatchContext.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSearch;
        private ComboBox cmbFilePattern;
        private TextBox txtContainingText;
        private CheckBox chkMatchCase;
        private CheckBox chkMatchWholeWord;
        private Label label1;
        private Label label2;
        private Label label3;
        private ContextMenuStrip mnuFileItemContext;
        private ToolStripMenuItem mnuCopyPath;
        private SplitContainer spltResultPanel;
        private SearchResultDataGridView grdResult;
        private ListView lstTextMatches;
        private Label label4;
        private ComboBox cmbExceptFilePattern;
        private ToolStripMenuItem mnuShowInExplorer;
        private CheckBox chkEnableExclusions;
        private ToolStripMenuItem mnuOpenWithTextEditor;
        private ToolStripMenuItem mnuOpenFile;
        private Button btnCancelSearch;
        private CheckBox chkFilterByDate;
        private TextBox txtDateFilterAmount;
        private ComboBox cmbDateFilterAmountType;
        private PredictiveComboBox cmbRootDir;
        private ToolStripMenuItem mnuCopyFiles;
        private ToolStripMenuItem mnuSearchHere;
        private ToolStripMenuItem mnuCopyFolderPath;
        private ContextMenuStrip mnuTextMatchContext;
        private ToolStripMenuItem mnuTextMatchLineCopy;
        private CheckBox chkShowHidden;
        private ToolStripMenuItem mnuExcludeThisFolder;
        private DataGridViewImageColumn colIcon;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colLocation;
        private DataGridViewTextBoxColumn colFileType;
        private DataGridViewTextBoxColumn colSize;
        private DataGridViewTextBoxColumn colLastModified;
    }
}
