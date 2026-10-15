namespace SharpExplorer
{
    partial class SearchForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabsMain = new TabControl();
            tabPage1 = new TabPage();
            mainForm3 = new SearchTab();
            mainForm1 = new SearchTab();
            mainForm2 = new SearchTab();
            tabsMain.SuspendLayout();
            tabPage1.SuspendLayout();
            SuspendLayout();
            // 
            // tabsMain
            // 
            tabsMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabsMain.Controls.Add(tabPage1);
            tabsMain.Location = new Point(5, 12);
            tabsMain.Name = "tabsMain";
            tabsMain.SelectedIndex = 0;
            tabsMain.Size = new Size(1314, 692);
            tabsMain.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(mainForm3);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1306, 659);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Search 1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // mainForm3
            // 
            mainForm3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            mainForm3.Location = new Point(4, 6);
            mainForm3.Name = "mainForm3";
            mainForm3.Size = new Size(1297, 647);
            mainForm3.TabIndex = 0;
            // 
            // mainForm1
            // 
            mainForm1.Location = new Point(160, 160);
            mainForm1.Name = "mainForm1";
            mainForm1.Size = new Size(1008, 564);
            mainForm1.TabIndex = 0;
            mainForm1.Visible = false;
            // 
            // mainForm2
            // 
            mainForm2.Location = new Point(192, 192);
            mainForm2.Name = "mainForm2";
            mainForm2.Size = new Size(1008, 564);
            mainForm2.TabIndex = 0;
            mainForm2.Visible = false;
            // 
            // SearchForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1325, 716);
            Controls.Add(tabsMain);
            Name = "SearchForm";
            Text = "Forager";
            tabsMain.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabsMain;
        private TabPage tabPage1;
        private SearchTab mainForm1;
        private SearchTab mainForm2;
        private SearchTab mainForm3;
    }
}