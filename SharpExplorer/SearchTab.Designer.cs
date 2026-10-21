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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            btnSearch = new Button();
            cmbFilePattern = new ComboBox();
            cmbContainingText = new ComboBox();
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
            statusStrip1 = new StatusStrip();
            TabProgressBar = new ToolStripProgressBar();
            lblTabStatus = new ToolStripStatusLabel();
            mnuFileItemContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)spltResultPanel).BeginInit();
            spltResultPanel.Panel1.SuspendLayout();
            spltResultPanel.Panel2.SuspendLayout();
            spltResultPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdResult).BeginInit();
            mnuTextMatchContext.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            //
            // btnSearch
            //
            btnSearch.Location = new Point(20, 142);
            btnSearch.Margin = new Padding(3, 2, 3, 2);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(82, 22);
            btnSearch.TabIndex = 14;
            btnSearch.Text = "&Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            //
            // cmbFilePattern
            //
            cmbFilePattern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbFilePattern.Location = new Point(108, 4);
            cmbFilePattern.Margin = new Padding(3, 2, 3, 2);
            cmbFilePattern.Name = "cmbFilePattern";
            cmbFilePattern.Size = new Size(766, 23);
            cmbFilePattern.TabIndex = 1;
            cmbFilePattern.Text = "*.txt";
            //
            // cmbContainingText
            //
            cmbContainingText.Anchor = AnchorStyles.Top;
            cmbContainingText.Location = new Point(109, 72);
            cmbContainingText.Margin = new Padding(3, 2, 3, 2);
            cmbContainingText.Name = "cmbContainingText";
            cmbContainingText.Size = new Size(638, 23);
            cmbContainingText.TabIndex = 5;
            //
            // mnuFileItemContext
            //
            mnuFileItemContext.ImageScalingSize = new Size(20, 20);
            mnuFileItemContext.Items.AddRange(new ToolStripItem[] { mnuOpenFile, mnuOpenWithTextEditor, mnuShowInExplorer, mnuCopyPath, mnuCopyFolderPath, mnuCopyFiles, mnuSearchHere, mnuExcludeThisFolder });
            mnuFileItemContext.Name = "mnuFileItemContext";
            mnuFileItemContext.Size = new Size(194, 180);
            //
            // mnuOpenFile
            //
            mnuOpenFile.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            mnuOpenFile.Name = "mnuOpenFile";
            mnuOpenFile.Size = new Size(193, 22);
            mnuOpenFile.Text = "Open";
            mnuOpenFile.Click += mnuOpenFile_Click;
            //
            // mnuOpenWithTextEditor
            //
            mnuOpenWithTextEditor.Name = "mnuOpenWithTextEditor";
            mnuOpenWithTextEditor.Size = new Size(193, 22);
            mnuOpenWithTextEditor.Text = "Open With Text Editor";
            mnuOpenWithTextEditor.Click += mnuOpenWithTextEditor_Click;
            //
            // mnuShowInExplorer
            //
            mnuShowInExplorer.Name = "mnuShowInExplorer";
            mnuShowInExplorer.Size = new Size(193, 22);
            mnuShowInExplorer.Text = "Open In File Explorer";
            mnuShowInExplorer.Click += mnuShowInExplorer_Click;
            //
            // mnuCopyPath
            //
            mnuCopyPath.Name = "mnuCopyPath";
            mnuCopyPath.ShortcutKeys = Keys.Control | Keys.C;
            mnuCopyPath.Size = new Size(193, 22);
            mnuCopyPath.Text = "Copy Full Path";
            mnuCopyPath.Click += mnuCopyPath_Click;
            //
            // mnuCopyFolderPath
            //
            mnuCopyFolderPath.Name = "mnuCopyFolderPath";
            mnuCopyFolderPath.Size = new Size(193, 22);
            mnuCopyFolderPath.Text = "Copy Folder Path";
            mnuCopyFolderPath.Click += mnuCopyFolderPath_Click;
            //
            // mnuCopyFiles
            //
            mnuCopyFiles.Name = "mnuCopyFiles";
            mnuCopyFiles.Size = new Size(193, 22);
            mnuCopyFiles.Text = "Copy";
            mnuCopyFiles.Click += mnuCopyFiles_Click;
            //
            // mnuSearchHere
            //
            mnuSearchHere.Name = "mnuSearchHere";
            mnuSearchHere.Size = new Size(193, 22);
            mnuSearchHere.Text = "Search This Folder...";
            mnuSearchHere.Click += mnuSearchHere_Click;
            //
            // mnuExcludeThisFolder
            //
            mnuExcludeThisFolder.Name = "mnuExcludeThisFolder";
            mnuExcludeThisFolder.Size = new Size(193, 22);
            mnuExcludeThisFolder.Text = "Exclude This Folder...";
            mnuExcludeThisFolder.Click += mnuExcludeThisFolder_Click;
            //
            // chkMatchCase
            //
            chkMatchCase.Anchor = AnchorStyles.Top;
            chkMatchCase.AutoSize = true;
            chkMatchCase.Location = new Point(757, 64);
            chkMatchCase.Margin = new Padding(3, 2, 3, 2);
            chkMatchCase.Name = "chkMatchCase";
            chkMatchCase.Size = new Size(88, 19);
            chkMatchCase.TabIndex = 11;
            chkMatchCase.Text = "Match &Case";
            chkMatchCase.UseVisualStyleBackColor = true;
            //
            // chkMatchWholeWord
            //
            chkMatchWholeWord.Anchor = AnchorStyles.Top;
            chkMatchWholeWord.AutoSize = true;
            chkMatchWholeWord.Location = new Point(757, 84);
            chkMatchWholeWord.Margin = new Padding(3, 2, 3, 2);
            chkMatchWholeWord.Name = "chkMatchWholeWord";
            chkMatchWholeWord.Size = new Size(92, 19);
            chkMatchWholeWord.TabIndex = 12;
            chkMatchWholeWord.Text = "Whole &Word";
            chkMatchWholeWord.UseVisualStyleBackColor = true;
            //
            // label1
            //
            label1.AutoSize = true;
            label1.Location = new Point(5, 6);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 0;
            label1.Text = "&File name:";
            //
            // label2
            //
            label2.AutoSize = true;
            label2.Location = new Point(5, 74);
            label2.Name = "label2";
            label2.Size = new Size(91, 15);
            label2.TabIndex = 4;
            label2.Text = "Containing &text:";
            //
            // label3
            //
            label3.AutoSize = true;
            label3.Location = new Point(5, 119);
            label3.Name = "label3";
            label3.Size = new Size(49, 15);
            label3.TabIndex = 6;
            label3.Text = "&Look in:";
            //
            // spltResultPanel
            //
            spltResultPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            spltResultPanel.FixedPanel = FixedPanel.Panel1;
            spltResultPanel.Location = new Point(4, 168);
            spltResultPanel.Margin = new Padding(3, 2, 3, 2);
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
            spltResultPanel.Size = new Size(874, 196);
            spltResultPanel.SplitterDistance = 621;
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
            grdResult.Margin = new Padding(3, 2, 3, 2);
            grdResult.Name = "grdResult";
            grdResult.ReadOnly = true;
            grdResult.RowHeadersVisible = false;
            grdResult.RowHeadersWidth = 51;
            grdResult.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdResult.ShowCellToolTips = false;
            grdResult.Size = new Size(621, 196);
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
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleRight;
            colSize.DefaultCellStyle = dataGridViewCellStyle1;
            colSize.HeaderText = "Size";
            colSize.MinimumWidth = 6;
            colSize.Name = "colSize";
            colSize.ReadOnly = true;
            colSize.Width = 52;
            //
            // colLastModified
            //
            colLastModified.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colLastModified.DataPropertyName = "LastModified";
            colLastModified.HeaderText = "Modified";
            colLastModified.MinimumWidth = 6;
            colLastModified.Name = "colLastModified";
            colLastModified.ReadOnly = true;
            colLastModified.Width = 80;
            //
            // lstTextMatches
            //
            lstTextMatches.ContextMenuStrip = mnuTextMatchContext;
            lstTextMatches.Dock = DockStyle.Fill;
            lstTextMatches.HeaderStyle = ColumnHeaderStyle.None;
            lstTextMatches.Location = new Point(0, 0);
            lstTextMatches.Margin = new Padding(3, 2, 3, 2);
            lstTextMatches.Name = "lstTextMatches";
            lstTextMatches.OwnerDraw = true;
            lstTextMatches.Size = new Size(249, 196);
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
            mnuTextMatchContext.Size = new Size(103, 26);
            //
            // mnuTextMatchLineCopy
            //
            mnuTextMatchLineCopy.Name = "mnuTextMatchLineCopy";
            mnuTextMatchLineCopy.Size = new Size(102, 22);
            mnuTextMatchLineCopy.Text = "Copy";
            mnuTextMatchLineCopy.Click += mnuTextMatchLineCopy_Click;
            //
            // label4
            //
            label4.AutoSize = true;
            label4.Location = new Point(5, 38);
            label4.Name = "label4";
            label4.Size = new Size(44, 15);
            label4.TabIndex = 2;
            label4.Text = "E&xcept:";
            //
            // cmbExceptFilePattern
            //
            cmbExceptFilePattern.Anchor = AnchorStyles.Top;
            cmbExceptFilePattern.FormattingEnabled = true;
            cmbExceptFilePattern.Location = new Point(109, 36);
            cmbExceptFilePattern.Margin = new Padding(3, 2, 3, 2);
            cmbExceptFilePattern.Name = "cmbExceptFilePattern";
            cmbExceptFilePattern.Size = new Size(676, 23);
            cmbExceptFilePattern.TabIndex = 3;
            cmbExceptFilePattern.Enter += txtExceptFilePattern_Enter;
            //
            // chkEnableExclusions
            //
            chkEnableExclusions.Anchor = AnchorStyles.Top;
            chkEnableExclusions.AutoSize = true;
            chkEnableExclusions.Location = new Point(789, 38);
            chkEnableExclusions.Margin = new Padding(3, 2, 3, 2);
            chkEnableExclusions.Name = "chkEnableExclusions";
            chkEnableExclusions.Size = new Size(61, 19);
            chkEnableExclusions.TabIndex = 13;
            chkEnableExclusions.Text = "E&nable";
            chkEnableExclusions.UseVisualStyleBackColor = true;
            chkEnableExclusions.CheckedChanged += chkEnableExclusions_CheckedChanged;
            //
            // btnCancelSearch
            //
            btnCancelSearch.Enabled = false;
            btnCancelSearch.Location = new Point(108, 142);
            btnCancelSearch.Margin = new Padding(3, 2, 3, 2);
            btnCancelSearch.Name = "btnCancelSearch";
            btnCancelSearch.Size = new Size(82, 22);
            btnCancelSearch.TabIndex = 15;
            btnCancelSearch.Text = "Sto&p";
            btnCancelSearch.UseVisualStyleBackColor = true;
            btnCancelSearch.Click += btnCancelSearch_Click;
            //
            // chkFilterByDate
            //
            chkFilterByDate.AutoSize = true;
            chkFilterByDate.Location = new Point(375, 146);
            chkFilterByDate.Margin = new Padding(3, 2, 3, 2);
            chkFilterByDate.Name = "chkFilterByDate";
            chkFilterByDate.Size = new Size(128, 19);
            chkFilterByDate.TabIndex = 8;
            chkFilterByDate.Text = "&Modified in the last";
            chkFilterByDate.UseVisualStyleBackColor = true;
            chkFilterByDate.CheckedChanged += chkFilterByDate_CheckedChanged;
            //
            // txtDateFilterAmount
            //
            txtDateFilterAmount.Location = new Point(509, 143);
            txtDateFilterAmount.Margin = new Padding(3, 2, 3, 2);
            txtDateFilterAmount.Name = "txtDateFilterAmount";
            txtDateFilterAmount.Size = new Size(38, 23);
            txtDateFilterAmount.TabIndex = 9;
            txtDateFilterAmount.TextAlign = HorizontalAlignment.Right;
            txtDateFilterAmount.Enter += txtDateFilterAmount_Enter;
            //
            // cmbDateFilterAmountType
            //
            cmbDateFilterAmountType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDateFilterAmountType.FormattingEnabled = true;
            cmbDateFilterAmountType.Items.AddRange(new object[] { "hours", "days", "weeks", "months", "years" });
            cmbDateFilterAmountType.Location = new Point(552, 143);
            cmbDateFilterAmountType.Margin = new Padding(3, 2, 3, 2);
            cmbDateFilterAmountType.Name = "cmbDateFilterAmountType";
            cmbDateFilterAmountType.Size = new Size(75, 23);
            cmbDateFilterAmountType.TabIndex = 10;
            //
            // cmbRootDir
            //
            cmbRootDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbRootDir.FormattingEnabled = true;
            cmbRootDir.Location = new Point(67, 117);
            cmbRootDir.Margin = new Padding(3, 2, 3, 2);
            cmbRootDir.Name = "cmbRootDir";
            cmbRootDir.Size = new Size(807, 23);
            cmbRootDir.TabIndex = 7;
            cmbRootDir.SelectedIndexChanged += cmbRootDir_SelectedIndexChanged;
            cmbRootDir.TextUpdate += cmbRootDir_TextUpdate;
            cmbRootDir.SelectedValueChanged += cmbRootDir_SelectedValueChanged;
            cmbRootDir.Enter += cmbRootDir_Enter;
            //
            // chkShowHidden
            //
            chkShowHidden.AutoSize = true;
            chkShowHidden.Location = new Point(738, 147);
            chkShowHidden.Name = "chkShowHidden";
            chkShowHidden.Size = new Size(97, 19);
            chkShowHidden.TabIndex = 104;
            chkShowHidden.Text = "Hidden Items";
            chkShowHidden.UseVisualStyleBackColor = true;
            //
            // statusStrip1
            //
            statusStrip1.AutoSize = false;
            statusStrip1.Items.AddRange(new ToolStripItem[] { TabProgressBar, lblTabStatus });
            statusStrip1.Location = new Point(0, 366);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(884, 22);
            statusStrip1.TabIndex = 105;
            statusStrip1.Text = "statusStrip1";
            //
            // TabProgressBar
            //
            TabProgressBar.Name = "TabProgressBar";
            TabProgressBar.Size = new Size(100, 16);
            TabProgressBar.Visible = false;
            //
            // lblTabStatus
            //
            lblTabStatus.Name = "lblTabStatus";
            lblTabStatus.Size = new Size(0, 17);
            //
            // SearchTab
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(statusStrip1);
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
            Controls.Add(cmbContainingText);
            Controls.Add(cmbFilePattern);
            Controls.Add(btnSearch);
            Margin = new Padding(3, 2, 3, 2);
            Name = "SearchTab";
            Size = new Size(884, 388);
            Load += SearchTab_Load;
            Resize += SearchTab_Resize;
            mnuFileItemContext.ResumeLayout(false);
            spltResultPanel.Panel1.ResumeLayout(false);
            spltResultPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)spltResultPanel).EndInit();
            spltResultPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdResult).EndInit();
            mnuTextMatchContext.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSearch;
        private ComboBox cmbFilePattern;
        private ComboBox cmbContainingText;
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
        private StatusStrip statusStrip1;
        private ToolStripProgressBar TabProgressBar;
        private ToolStripStatusLabel lblTabStatus;
    }
}
