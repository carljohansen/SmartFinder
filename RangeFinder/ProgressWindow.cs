using RangeFinder.Domain;

namespace RangeFinder;

public class ProgressWindow : Form
{
    private readonly ProgressBar _progressBar;
    private readonly Label _statusLabel;

    private readonly string _zipFilePath;

    public string? ExtractedDirectoryPath { get; private set; }

    public ProgressWindow(string zipFilePath)
    {
        _zipFilePath = zipFilePath;

        // Window setup
        Text = "Explore Zip Archive";
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
            Text = "Extracting..."
        };

        Controls.Add(_statusLabel);
        Controls.Add(_progressBar);

        Load += ProgressWindow_Load;
    }

    private async void ProgressWindow_Load(object? sender, EventArgs e)
    {
        var progressHandler = new Progress<ExtractionProgressEventArgs>(args =>
        {
            _progressBar.Value = args.ProgressPercentage;
        });

        ExtractedDirectoryPath = await ZipExpander.Instance.UnzipToTempDirectoryAsync(_zipFilePath, progressHandler, CancellationToken.None);

        DialogResult = ExtractedDirectoryPath == null ? DialogResult.Cancel : DialogResult.OK;
        Close();
    }
}
