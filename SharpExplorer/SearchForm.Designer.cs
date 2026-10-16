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
            statusStrip1 = new StatusStrip();
            MainProgressBar = new ToolStripProgressBar();
            lblFormStatusMessage = new ToolStripStatusLabel();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            mnuQuit = new ToolStripMenuItem();
            searchToolStripMenuItem = new ToolStripMenuItem();
            mnuNewSearchTab = new ToolStripMenuItem();
            closeTabToolStripMenuItem = new ToolStripMenuItem();
            duplicateTabToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            //
            // tabsMain
            //
            tabsMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabsMain.Location = new Point(5, 33);
            tabsMain.Name = "tabsMain";
            tabsMain.SelectedIndex = 0;
            tabsMain.Size = new Size(1314, 630);
            tabsMain.TabIndex = 0;
            //
            // statusStrip1
            //
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { MainProgressBar, lblFormStatusMessage });
            statusStrip1.Location = new Point(0, 671);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1325, 24);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            //
            // MainProgressBar
            //
            MainProgressBar.Margin = new Padding(11, 4, 1, 4);
            MainProgressBar.Name = "MainProgressBar";
            MainProgressBar.Size = new Size(100, 16);
            //
            // lblFormStatusMessage
            //
            lblFormStatusMessage.Name = "lblFormStatusMessage";
            lblFormStatusMessage.Size = new Size(0, 18);
            //
            // menuStrip1
            //
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, searchToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1325, 28);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            //
            // fileToolStripMenuItem
            //
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mnuQuit });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            //
            // mnuQuit
            //
            mnuQuit.Name = "mnuQuit";
            mnuQuit.Size = new Size(224, 26);
            mnuQuit.Text = "E&xit";
            mnuQuit.Click += mnuQuit_Click;
            //
            // searchToolStripMenuItem
            //
            searchToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mnuNewSearchTab, closeTabToolStripMenuItem, duplicateTabToolStripMenuItem });
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.Size = new Size(67, 24);
            searchToolStripMenuItem.Text = "Search";
            //
            // mnuNewSearchTab
            //
            mnuNewSearchTab.Name = "mnuNewSearchTab";
            mnuNewSearchTab.ShortcutKeys = Keys.Control | Keys.T;
            mnuNewSearchTab.Size = new Size(274, 26);
            mnuNewSearchTab.Text = "New Tab";
            mnuNewSearchTab.Click += mnuNewSearchTab_Click;
            //
            // closeTabToolStripMenuItem
            //
            closeTabToolStripMenuItem.Name = "closeTabToolStripMenuItem";
            closeTabToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.W;
            closeTabToolStripMenuItem.Size = new Size(274, 26);
            closeTabToolStripMenuItem.Text = "Close Tab";
            closeTabToolStripMenuItem.Click += closeTabToolStripMenuItem_Click;
            //
            // duplicateTabToolStripMenuItem
            //
            duplicateTabToolStripMenuItem.Name = "duplicateTabToolStripMenuItem";
            duplicateTabToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.K;
            duplicateTabToolStripMenuItem.Size = new Size(274, 26);
            duplicateTabToolStripMenuItem.Text = "Duplicate Tab";
            duplicateTabToolStripMenuItem.Click += duplicateTabToolStripMenuItem_Click;
            //
            // SearchForm
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1325, 695);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            Controls.Add(tabsMain);
            MainMenuStrip = menuStrip1;
            Name = "SearchForm";
            Text = "Forager";
            Load += SearchForm_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl tabsMain;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblFormStatusMessage;
        private ToolStripProgressBar MainProgressBar;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem mnuQuit;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripMenuItem mnuNewSearchTab;
        private ToolStripMenuItem closeTabToolStripMenuItem;
        private ToolStripMenuItem duplicateTabToolStripMenuItem;
    }
}
