namespace SharpExplorer
{
    partial class Form1
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
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            btnSearch = new Button();
            txtFilePattern = new TextBox();
            txtContainingText = new TextBox();
            txtRootDir = new TextBox();
            mnuFileItemContext = new ContextMenuStrip(components);
            mnuCopyPath = new ToolStripMenuItem();
            chkMatchCase = new CheckBox();
            chkMatchWholeWord = new CheckBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            spltResultPanel = new SplitContainer();
            grdResult = new DataGridView();
            colName = new DataGridViewTextBoxColumn();
            colLocation = new DataGridViewTextBoxColumn();
            colSize = new DataGridViewTextBoxColumn();
            colLastModified = new DataGridViewTextBoxColumn();
            lstTextMatches = new ListView();
            mnuFileItemContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)spltResultPanel).BeginInit();
            spltResultPanel.Panel1.SuspendLayout();
            spltResultPanel.Panel2.SuspendLayout();
            spltResultPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdResult).BeginInit();
            SuspendLayout();
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(23, 219);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(94, 29);
            btnSearch.TabIndex = 8;
            btnSearch.Text = "&Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // txtFilePattern
            // 
            txtFilePattern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFilePattern.Location = new Point(140, 39);
            txtFilePattern.Name = "txtFilePattern";
            txtFilePattern.Size = new Size(834, 27);
            txtFilePattern.TabIndex = 1;
            txtFilePattern.Text = "**/*.txt";
            // 
            // txtContainingText
            // 
            txtContainingText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtContainingText.Location = new Point(140, 98);
            txtContainingText.Name = "txtContainingText";
            txtContainingText.Size = new Size(714, 27);
            txtContainingText.TabIndex = 3;
            txtContainingText.Enter += txtContainingText_Enter;
            // 
            // txtRootDir
            // 
            txtRootDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRootDir.Location = new Point(140, 158);
            txtRootDir.Name = "txtRootDir";
            txtRootDir.Size = new Size(834, 27);
            txtRootDir.TabIndex = 7;
            txtRootDir.Text = "S:\\Text";
            // 
            // mnuFileItemContext
            // 
            mnuFileItemContext.ImageScalingSize = new Size(20, 20);
            mnuFileItemContext.Items.AddRange(new ToolStripItem[] { mnuCopyPath });
            mnuFileItemContext.Name = "mnuFileItemContext";
            mnuFileItemContext.Size = new Size(172, 28);
            // 
            // mnuCopyPath
            // 
            mnuCopyPath.Name = "mnuCopyPath";
            mnuCopyPath.Size = new Size(171, 24);
            mnuCopyPath.Text = "Copy Full Path";
            mnuCopyPath.Click += mnuCopyPath_Click;
            // 
            // chkMatchCase
            // 
            chkMatchCase.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkMatchCase.AutoSize = true;
            chkMatchCase.Location = new Point(864, 83);
            chkMatchCase.Name = "chkMatchCase";
            chkMatchCase.Size = new Size(107, 24);
            chkMatchCase.TabIndex = 4;
            chkMatchCase.Text = "Match &Case";
            chkMatchCase.UseVisualStyleBackColor = true;
            // 
            // chkMatchWholeWord
            // 
            chkMatchWholeWord.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkMatchWholeWord.AutoSize = true;
            chkMatchWholeWord.Location = new Point(864, 119);
            chkMatchWholeWord.Name = "chkMatchWholeWord";
            chkMatchWholeWord.Size = new Size(114, 24);
            chkMatchWholeWord.TabIndex = 5;
            chkMatchWholeWord.Text = "Whole &Word";
            chkMatchWholeWord.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(23, 46);
            label1.Name = "label1";
            label1.Size = new Size(76, 20);
            label1.TabIndex = 0;
            label1.Text = "&File name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(22, 101);
            label2.Name = "label2";
            label2.Size = new Size(113, 20);
            label2.TabIndex = 2;
            label2.Text = "Containing &text:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(22, 158);
            label3.Name = "label3";
            label3.Size = new Size(60, 20);
            label3.TabIndex = 6;
            label3.Text = "&Look in:";
            // 
            // spltResultPanel
            // 
            spltResultPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
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
            spltResultPanel.Size = new Size(984, 298);
            spltResultPanel.SplitterDistance = 710;
            spltResultPanel.TabIndex = 101;
            // 
            // grdResult
            // 
            grdResult.AllowUserToAddRows = false;
            grdResult.AllowUserToDeleteRows = false;
            grdResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grdResult.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdResult.Columns.AddRange(new DataGridViewColumn[] { colName, colLocation, colSize, colLastModified });
            grdResult.ContextMenuStrip = mnuFileItemContext;
            grdResult.Location = new Point(0, 3);
            grdResult.Name = "grdResult";
            grdResult.ReadOnly = true;
            grdResult.RowHeadersWidth = 51;
            grdResult.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdResult.Size = new Size(707, 292);
            grdResult.TabIndex = 100;
            grdResult.CellContextMenuStripNeeded += grdResult_CellContextMenuStripNeeded;
            grdResult.RowEnter += grdResult_RowEnter;
            // 
            // colName
            // 
            colName.HeaderText = "Name";
            colName.MinimumWidth = 6;
            colName.Name = "colName";
            colName.ReadOnly = true;
            colName.Width = 125;
            // 
            // colLocation
            // 
            colLocation.HeaderText = "Location";
            colLocation.MinimumWidth = 6;
            colLocation.Name = "colLocation";
            colLocation.ReadOnly = true;
            colLocation.Width = 125;
            // 
            // colSize
            // 
            colSize.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleRight;
            colSize.DefaultCellStyle = dataGridViewCellStyle3;
            colSize.HeaderText = "Size";
            colSize.MinimumWidth = 6;
            colSize.Name = "colSize";
            colSize.ReadOnly = true;
            colSize.Width = 65;
            // 
            // colLastModified
            // 
            colLastModified.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colLastModified.HeaderText = "Modified";
            colLastModified.MinimumWidth = 6;
            colLastModified.Name = "colLastModified";
            colLastModified.ReadOnly = true;
            colLastModified.Width = 200;
            // 
            // lstTextMatches
            // 
            lstTextMatches.Dock = DockStyle.Fill;
            lstTextMatches.Location = new Point(0, 0);
            lstTextMatches.Name = "lstTextMatches";
            lstTextMatches.OwnerDraw = true;
            lstTextMatches.Size = new Size(270, 298);
            lstTextMatches.TabIndex = 102;
            lstTextMatches.UseCompatibleStateImageBehavior = false;
            lstTextMatches.View = View.List;
            lstTextMatches.DrawItem += lstTextMatches_DrawItem;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1008, 564);
            Controls.Add(spltResultPanel);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(chkMatchWholeWord);
            Controls.Add(chkMatchCase);
            Controls.Add(txtRootDir);
            Controls.Add(txtContainingText);
            Controls.Add(txtFilePattern);
            Controls.Add(btnSearch);
            Name = "Form1";
            Text = "Sharp Explorer";
            Load += Form1_Load;
            mnuFileItemContext.ResumeLayout(false);
            spltResultPanel.Panel1.ResumeLayout(false);
            spltResultPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)spltResultPanel).EndInit();
            spltResultPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdResult).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSearch;
        private TextBox txtFilePattern;
        private TextBox txtContainingText;
        private TextBox txtRootDir;
        private CheckBox chkMatchCase;
        private CheckBox chkMatchWholeWord;
        private Label label1;
        private Label label2;
        private Label label3;
        private ContextMenuStrip mnuFileItemContext;
        private ToolStripMenuItem mnuCopyPath;
        private SplitContainer spltResultPanel;
        private DataGridView grdResult;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colLocation;
        private DataGridViewTextBoxColumn colSize;
        private DataGridViewTextBoxColumn colLastModified;
        private ListView lstTextMatches;
    }
}
