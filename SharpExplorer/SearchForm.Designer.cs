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
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            mnuQuit = new ToolStripMenuItem();
            mnuClearSearchHistory = new ToolStripMenuItem();
            searchToolStripMenuItem = new ToolStripMenuItem();
            mnuNewSearchTab = new ToolStripMenuItem();
            closeTabToolStripMenuItem = new ToolStripMenuItem();
            duplicateTabToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            //
            // tabsMain
            //
            tabsMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabsMain.Location = new Point(0, 26);
            tabsMain.Margin = new Padding(3, 2, 3, 2);
            tabsMain.Name = "tabsMain";
            tabsMain.SelectedIndex = 0;
            tabsMain.Size = new Size(1159, 495);
            tabsMain.TabIndex = 0;
            //
            // menuStrip1
            //
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, searchToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(5, 2, 0, 2);
            menuStrip1.Size = new Size(1159, 24);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            //
            // fileToolStripMenuItem
            //
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mnuClearSearchHistory, mnuQuit });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            //
            // mnuQuit
            //
            mnuQuit.Name = "mnuQuit";
            mnuQuit.Size = new Size(180, 22);
            mnuQuit.Text = "E&xit";
            mnuQuit.Click += mnuQuit_Click;
            //
            // mnuClearSearchHistory
            //
            mnuClearSearchHistory.Name = "mnuClearSearchHistory";
            mnuClearSearchHistory.Size = new Size(180, 22);
            mnuClearSearchHistory.Text = "Clear Search &History";
            mnuClearSearchHistory.Click += mnuClearSearchHistory_Click;
            //
            // searchToolStripMenuItem
            //
            searchToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mnuNewSearchTab, closeTabToolStripMenuItem, duplicateTabToolStripMenuItem });
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.Size = new Size(54, 20);
            searchToolStripMenuItem.Text = "Search";
            //
            // mnuNewSearchTab
            //
            mnuNewSearchTab.Name = "mnuNewSearchTab";
            mnuNewSearchTab.ShortcutKeys = Keys.Control | Keys.T;
            mnuNewSearchTab.Size = new Size(219, 22);
            mnuNewSearchTab.Text = "New Tab";
            mnuNewSearchTab.Click += mnuNewSearchTab_Click;
            //
            // closeTabToolStripMenuItem
            //
            closeTabToolStripMenuItem.Name = "closeTabToolStripMenuItem";
            closeTabToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.W;
            closeTabToolStripMenuItem.Size = new Size(219, 22);
            closeTabToolStripMenuItem.Text = "Close Tab";
            closeTabToolStripMenuItem.Click += closeTabToolStripMenuItem_Click;
            //
            // duplicateTabToolStripMenuItem
            //
            duplicateTabToolStripMenuItem.Name = "duplicateTabToolStripMenuItem";
            duplicateTabToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.K;
            duplicateTabToolStripMenuItem.Size = new Size(219, 22);
            duplicateTabToolStripMenuItem.Text = "Duplicate Tab";
            duplicateTabToolStripMenuItem.Click += duplicateTabToolStripMenuItem_Click;
            //
            // SearchForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1159, 521);
            Controls.Add(menuStrip1);
            Controls.Add(tabsMain);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(3, 2, 3, 2);
            Name = "SearchForm";
            Text = "RangeFinder";
            FormClosing += SearchForm_FormClosing;
            Load += SearchForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl tabsMain;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem mnuQuit;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripMenuItem mnuNewSearchTab;
        private ToolStripMenuItem closeTabToolStripMenuItem;
        private ToolStripMenuItem duplicateTabToolStripMenuItem;
        private ToolStripMenuItem mnuClearSearchHistory;
    }
}
