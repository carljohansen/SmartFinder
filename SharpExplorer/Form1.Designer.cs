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
            btnSearch = new Button();
            txtFilePattern = new TextBox();
            txtContainingText = new TextBox();
            txtRootDir = new TextBox();
            grdResult = new DataGridView();
            colName = new DataGridViewTextBoxColumn();
            colLocation = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)grdResult).BeginInit();
            SuspendLayout();
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(88, 209);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(94, 29);
            btnSearch.TabIndex = 3;
            btnSearch.Text = "&Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // txtFilePattern
            // 
            txtFilePattern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFilePattern.Location = new Point(88, 39);
            txtFilePattern.Name = "txtFilePattern";
            txtFilePattern.Size = new Size(886, 27);
            txtFilePattern.TabIndex = 0;
            txtFilePattern.Text = "**/*.jpg";
            // 
            // txtContainingText
            // 
            txtContainingText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtContainingText.Location = new Point(88, 98);
            txtContainingText.Name = "txtContainingText";
            txtContainingText.Size = new Size(886, 27);
            txtContainingText.TabIndex = 1;
            // 
            // txtRootDir
            // 
            txtRootDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRootDir.Location = new Point(88, 158);
            txtRootDir.Name = "txtRootDir";
            txtRootDir.Size = new Size(886, 27);
            txtRootDir.TabIndex = 2;
            txtRootDir.Text = "S:\\mod";
            // 
            // grdResult
            // 
            grdResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grdResult.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdResult.Columns.AddRange(new DataGridViewColumn[] { colName, colLocation });
            grdResult.Location = new Point(22, 269);
            grdResult.Name = "grdResult";
            grdResult.ReadOnly = true;
            grdResult.RowHeadersWidth = 51;
            grdResult.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdResult.Size = new Size(974, 286);
            grdResult.TabIndex = 4;
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1008, 580);
            Controls.Add(grdResult);
            Controls.Add(txtRootDir);
            Controls.Add(txtContainingText);
            Controls.Add(txtFilePattern);
            Controls.Add(btnSearch);
            Name = "Form1";
            Text = "Sharp Explorer";
            ((System.ComponentModel.ISupportInitialize)grdResult).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSearch;
        private TextBox txtFilePattern;
        private TextBox txtContainingText;
        private TextBox txtRootDir;
        private DataGridView grdResult;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colLocation;
    }
}
