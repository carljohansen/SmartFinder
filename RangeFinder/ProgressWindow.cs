using System.ComponentModel;

namespace RangeFinder;

public class ProgressWindow : Form
{
    private ProgressBar _progressBar;
    private Label _statusLabel;
    private BackgroundWorker _worker;

    public ProgressWindow(Action<BackgroundWorker> longTask)
    {
        // Window setup
        Text = "Processing...";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        TopMost = true;
        Size = new System.Drawing.Size(300, 100);
        MaximizeBox = false;
        MinimizeBox = false;

        // Progress bar
        _progressBar = new ProgressBar
        {
            Dock = DockStyle.Bottom,
            Minimum = 0,
            Maximum = 100
        };

        // Status label
        _statusLabel = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
            Text = "Please wait..."
        };

        Controls.Add(_statusLabel);
        Controls.Add(_progressBar);

        // Background worker setup
        _worker = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };
        _worker.DoWork += (s, e) => longTask(_worker);
        _worker.ProgressChanged += (s, e) =>
        {
            _progressBar.Value = e.ProgressPercentage;
            _statusLabel.Text = $"Progress: {e.ProgressPercentage}%";
        };
        _worker.RunWorkerCompleted += (s, e) =>
        {
            DialogResult = DialogResult.OK;
            Close();
        };
    }

    public void Start()
    {
        _worker.RunWorkerAsync();
        ShowDialog();
    }
}
