namespace SharpExplorer
{
    partial class MainForm
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
            txtFilePattern = new TextBox();
            txtContainingText = new TextBox();
            mnuFileItemContext = new ContextMenuStrip(components);
            mnuOpenFile = new ToolStripMenuItem();
            mnuCopyPath = new ToolStripMenuItem();
            mnuShowInExplorer = new ToolStripMenuItem();
            mnuOpenWithTextEditor = new ToolStripMenuItem();
            chkMatchCase = new CheckBox();
            chkMatchWholeWord = new CheckBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            spltResultPanel = new SplitContainer();
            grdResult = new DataGridView();
            colIcon = new DataGridViewImageColumn();
            colName = new DataGridViewTextBoxColumn();
            colLocation = new DataGridViewTextBoxColumn();
            colFileType = new DataGridViewTextBoxColumn();
            colSize = new DataGridViewTextBoxColumn();
            colLastModified = new DataGridViewTextBoxColumn();
            lstTextMatches = new ListView();
            label4 = new Label();
            txtExceptFilePattern = new TextBox();
            chkEnableExclusions = new CheckBox();
            statusStrip1 = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            btnCancelSearch = new Button();
            chkFilterByDate = new CheckBox();
            txtDateFilterAmount = new TextBox();
            cmbDateFilterAmountType = new ComboBox();
            cmbRootDir = new ComboBox();
            mnuFileItemContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)spltResultPanel).BeginInit();
            spltResultPanel.Panel1.SuspendLayout();
            spltResultPanel.Panel2.SuspendLayout();
            spltResultPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdResult).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(23, 219);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(94, 29);
            btnSearch.TabIndex = 14;
            btnSearch.Text = "&Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // txtFilePattern
            // 
            txtFilePattern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFilePattern.Location = new Point(140, 35);
            txtFilePattern.Name = "txtFilePattern";
            txtFilePattern.Size = new Size(834, 27);
            txtFilePattern.TabIndex = 1;
            txtFilePattern.Text = "*.txt";
            txtFilePattern.Enter += txtFilePattern_Enter;
            // 
            // txtContainingText
            // 
            txtContainingText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtContainingText.Location = new Point(140, 125);
            txtContainingText.Name = "txtContainingText";
            txtContainingText.Size = new Size(714, 27);
            txtContainingText.TabIndex = 5;
            txtContainingText.Enter += txtContainingText_Enter;
            // 
            // mnuFileItemContext
            // 
            mnuFileItemContext.ImageScalingSize = new Size(20, 20);
            mnuFileItemContext.Items.AddRange(new ToolStripItem[] { mnuOpenFile, mnuCopyPath, mnuShowInExplorer, mnuOpenWithTextEditor });
            mnuFileItemContext.Name = "mnuFileItemContext";
            mnuFileItemContext.Size = new Size(237, 100);
            // 
            // mnuOpenFile
            // 
            mnuOpenFile.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            mnuOpenFile.Name = "mnuOpenFile";
            mnuOpenFile.Size = new Size(236, 24);
            mnuOpenFile.Text = "Open";
            mnuOpenFile.Click += mnuOpenFile_Click;
            // 
            // mnuCopyPath
            // 
            mnuCopyPath.Name = "mnuCopyPath";
            mnuCopyPath.Size = new Size(236, 24);
            mnuCopyPath.Text = "Copy Full Path";
            mnuCopyPath.Click += mnuCopyPath_Click;
            // 
            // mnuShowInExplorer
            // 
            mnuShowInExplorer.Name = "mnuShowInExplorer";
            mnuShowInExplorer.Size = new Size(236, 24);
            mnuShowInExplorer.Text = "Open Containing Folder";
            mnuShowInExplorer.Click += mnuShowInExplorer_Click;
            // 
            // mnuOpenWithTextEditor
            // 
            mnuOpenWithTextEditor.Name = "mnuOpenWithTextEditor";
            mnuOpenWithTextEditor.Size = new Size(236, 24);
            mnuOpenWithTextEditor.Text = "Open With Text Editor";
            mnuOpenWithTextEditor.Click += mnuOpenWithTextEditor_Click;
            // 
            // chkMatchCase
            // 
            chkMatchCase.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkMatchCase.AutoSize = true;
            chkMatchCase.Location = new Point(864, 115);
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
            chkMatchWholeWord.Location = new Point(864, 141);
            chkMatchWholeWord.Name = "chkMatchWholeWord";
            chkMatchWholeWord.Size = new Size(114, 24);
            chkMatchWholeWord.TabIndex = 12;
            chkMatchWholeWord.Text = "Whole &Word";
            chkMatchWholeWord.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 38);
            label1.Name = "label1";
            label1.Size = new Size(76, 20);
            label1.TabIndex = 0;
            label1.Text = "&File name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(27, 128);
            label2.Name = "label2";
            label2.Size = new Size(113, 20);
            label2.TabIndex = 4;
            label2.Text = "Containing &text:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(27, 188);
            label3.Name = "label3";
            label3.Size = new Size(60, 20);
            label3.TabIndex = 6;
            label3.Text = "&Look in:";
            // 
            // spltResultPanel
            // 
            spltResultPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            spltResultPanel.FixedPanel = FixedPanel.Panel1;
            spltResultPanel.Location = new Point(12, 254);
            spltResultPanel.Name = "spltResultPanel";
            // 
            // spltResultPanel.Panel1
            // 
            spltResultPanel.Panel1.Controls.Add(grdResult);
            // 
            // spltResultPanel.Panel2
            // 
            spltResultPanel.Panel2.Controls.Add(lstTextMatches);
            spltResultPanel.Size = new Size(984, 268);
            spltResultPanel.SplitterDistance = 710;
            spltResultPanel.TabIndex = 101;
            // 
            // grdResult
            // 
            grdResult.AllowUserToAddRows = false;
            grdResult.AllowUserToDeleteRows = false;
            grdResult.AllowUserToResizeRows = false;
            grdResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grdResult.CellBorderStyle = DataGridViewCellBorderStyle.None;
            grdResult.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdResult.Columns.AddRange(new DataGridViewColumn[] { colIcon, colName, colLocation, colFileType, colSize, colLastModified });
            grdResult.ContextMenuStrip = mnuFileItemContext;
            grdResult.Location = new Point(0, 3);
            grdResult.Name = "grdResult";
            grdResult.ReadOnly = true;
            grdResult.RowHeadersVisible = false;
            grdResult.RowHeadersWidth = 51;
            grdResult.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdResult.Size = new Size(707, 262);
            grdResult.TabIndex = 100;
            grdResult.CellContextMenuStripNeeded += grdResult_CellContextMenuStripNeeded;
            grdResult.CellMouseDoubleClick += grdResult_CellMouseDoubleClick;
            grdResult.RowEnter += grdResult_RowEnter;
            // 
            // colIcon
            // 
            colIcon.HeaderText = "";
            colIcon.MinimumWidth = 6;
            colIcon.Name = "colIcon";
            colIcon.ReadOnly = true;
            colIcon.Width = 125;
            // 
            // colName
            // 
            colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colName.DataPropertyName = "Name";
            colName.HeaderText = "Name";
            colName.MinimumWidth = 6;
            colName.Name = "colName";
            colName.ReadOnly = true;
            colName.Width = 78;
            // 
            // colLocation
            // 
            colLocation.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colLocation.DataPropertyName = "Location";
            colLocation.HeaderText = "Location";
            colLocation.MinimumWidth = 6;
            colLocation.Name = "colLocation";
            colLocation.ReadOnly = true;
            colLocation.Width = 95;
            // 
            // colFileType
            // 
            colFileType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
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
            lstTextMatches.Dock = DockStyle.Fill;
            lstTextMatches.HeaderStyle = ColumnHeaderStyle.None;
            lstTextMatches.Location = new Point(0, 0);
            lstTextMatches.Name = "lstTextMatches";
            lstTextMatches.OwnerDraw = true;
            lstTextMatches.Size = new Size(270, 268);
            lstTextMatches.TabIndex = 102;
            lstTextMatches.UseCompatibleStateImageBehavior = false;
            lstTextMatches.View = View.Details;
            lstTextMatches.DrawItem += lstTextMatches_DrawItem;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(27, 80);
            label4.Name = "label4";
            label4.Size = new Size(56, 20);
            label4.TabIndex = 2;
            label4.Text = "E&xcept:";
            // 
            // txtExceptFilePattern
            // 
            txtExceptFilePattern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtExceptFilePattern.Location = new Point(140, 77);
            txtExceptFilePattern.Name = "txtExceptFilePattern";
            txtExceptFilePattern.Size = new Size(748, 27);
            txtExceptFilePattern.TabIndex = 3;
            txtExceptFilePattern.Enter += txtExceptFilePattern_Enter;
            // 
            // chkEnableExclusions
            // 
            chkEnableExclusions.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkEnableExclusions.AutoSize = true;
            chkEnableExclusions.Location = new Point(902, 79);
            chkEnableExclusions.Name = "chkEnableExclusions";
            chkEnableExclusions.Size = new Size(76, 24);
            chkEnableExclusions.TabIndex = 13;
            chkEnableExclusions.Text = "Enable";
            chkEnableExclusions.UseVisualStyleBackColor = true;
            chkEnableExclusions.CheckedChanged += chkEnableExclusions_CheckedChanged;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus });
            statusStrip1.Location = new Point(0, 542);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1008, 22);
            statusStrip1.TabIndex = 103;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 16);
            lblStatus.Click += toolStripStatusLabel1_Click;
            // 
            // btnCancelSearch
            // 
            btnCancelSearch.Enabled = false;
            btnCancelSearch.Location = new Point(123, 219);
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
            chkFilterByDate.Location = new Point(419, 222);
            chkFilterByDate.Name = "chkFilterByDate";
            chkFilterByDate.Size = new Size(160, 24);
            chkFilterByDate.TabIndex = 8;
            chkFilterByDate.Text = "&Modified in the last";
            chkFilterByDate.UseVisualStyleBackColor = true;
            chkFilterByDate.CheckedChanged += chkFilterByDate_CheckedChanged;
            // 
            // txtDateFilterAmount
            // 
            txtDateFilterAmount.Location = new Point(585, 221);
            txtDateFilterAmount.Name = "txtDateFilterAmount";
            txtDateFilterAmount.Size = new Size(43, 27);
            txtDateFilterAmount.TabIndex = 9;
            txtDateFilterAmount.Enter += txtDateFilterAmount_Enter;
            // 
            // cmbDateFilterAmountType
            // 
            cmbDateFilterAmountType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDateFilterAmountType.FormattingEnabled = true;
            cmbDateFilterAmountType.Items.AddRange(new object[] { "hours", "days", "months", "years" });
            cmbDateFilterAmountType.Location = new Point(634, 221);
            cmbDateFilterAmountType.Name = "cmbDateFilterAmountType";
            cmbDateFilterAmountType.Size = new Size(85, 28);
            cmbDateFilterAmountType.TabIndex = 10;
            // 
            // cmbRootDir
            // 
            cmbRootDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbRootDir.FormattingEnabled = true;
            cmbRootDir.Location = new Point(93, 185);
            cmbRootDir.Name = "cmbRootDir";
            cmbRootDir.Size = new Size(903, 28);
            cmbRootDir.TabIndex = 7;
            cmbRootDir.TextUpdate += cmbRootDir_TextUpdate;
            cmbRootDir.Enter += cmbRootDir_Enter;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1008, 564);
            Controls.Add(cmbRootDir);
            Controls.Add(cmbDateFilterAmountType);
            Controls.Add(txtDateFilterAmount);
            Controls.Add(chkFilterByDate);
            Controls.Add(btnCancelSearch);
            Controls.Add(statusStrip1);
            Controls.Add(chkEnableExclusions);
            Controls.Add(label4);
            Controls.Add(txtExceptFilePattern);
            Controls.Add(spltResultPanel);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(chkMatchWholeWord);
            Controls.Add(chkMatchCase);
            Controls.Add(txtContainingText);
            Controls.Add(txtFilePattern);
            Controls.Add(btnSearch);
            KeyPreview = true;
            Name = "MainForm";
            Text = "Sharp Explorer";
            Load += Form1_Load;
            mnuFileItemContext.ResumeLayout(false);
            spltResultPanel.Panel1.ResumeLayout(false);
            spltResultPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)spltResultPanel).EndInit();
            spltResultPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdResult).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSearch;
        private TextBox txtFilePattern;
        private TextBox txtContainingText;
        private CheckBox chkMatchCase;
        private CheckBox chkMatchWholeWord;
        private Label label1;
        private Label label2;
        private Label label3;
        private ContextMenuStrip mnuFileItemContext;
        private ToolStripMenuItem mnuCopyPath;
        private SplitContainer spltResultPanel;
        private DataGridView grdResult;
        private ListView lstTextMatches;
        private Label label4;
        private TextBox txtExceptFilePattern;
        private ToolStripMenuItem mnuShowInExplorer;
        private CheckBox chkEnableExclusions;
        private ToolStripMenuItem mnuOpenWithTextEditor;
        private ToolStripMenuItem mnuOpenFile;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblStatus;
        private Button btnCancelSearch;
        private CheckBox chkFilterByDate;
        private TextBox txtDateFilterAmount;
        private ComboBox cmbDateFilterAmountType;
        private DataGridViewImageColumn colIcon;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colLocation;
        private DataGridViewTextBoxColumn colFileType;
        private DataGridViewTextBoxColumn colSize;
        private DataGridViewTextBoxColumn colLastModified;
        private ComboBox cmbRootDir;
    }
}
