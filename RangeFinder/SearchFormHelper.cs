using RangeFinder.Domain;
using RangeFinder.Domain.History;

namespace RangeFinder;

internal static class SearchFormHelper
{
    public static SearchTab CreateSearchTabControl(UserTabState initialState) => new(initialState)
    {
        Dock = DockStyle.Fill
    };

    public static bool IsOnScreen(Rectangle rect)
    {
        foreach (Screen screen in Screen.AllScreens)
        {
            if (screen.WorkingArea.IntersectsWith(rect))
                return true;
        }
        return false;
    }

    public static string GetLastZipLocation()
    {
        var result = Properties.Settings.Default.LastZipLocation;
        if (string.IsNullOrWhiteSpace(result) || !Directory.Exists(result))
        {
            result = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
        return result;
    }

    public static async Task<string?> GetDraggedZipFileNameAsync(DragEventArgs e)
    {
        if (!e.Data?.GetDataPresent(DataFormats.FileDrop) ?? false)
        {
            return null;
        }
        string[] fileNames = (string[])(e.Data?.GetData(DataFormats.FileDrop) ?? Array.Empty<string>());
        if (fileNames.Length != 1)
        {
            return null;
        }

        var fileName = fileNames[0];
        return ZipExpander.VerifyZipFileByContent(fileName);
    }

    public static void DrawTabHeaders(TabControl tabControl, DrawItemEventArgs e)
    {
        //e.DrawBackground();
        var tabPage = tabControl.TabPages[e.Index] as SearchTabPage;
        using Brush br = new SolidBrush(tabPage?.HeaderColor ?? default);
        e.Graphics.FillRectangle(br, e.Bounds);
        SizeF sz = e.Graphics.MeasureString(tabControl.TabPages[e.Index].Text, e.Font!);
        e.Graphics.DrawString(tabControl.TabPages[e.Index].Text, e.Font!, Brushes.Black, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1);

        if (e.Index != tabControl.SelectedIndex)
        {
            return;
        }

        Rectangle rect = e.Bounds;
        rect.Offset(0, 1);
        rect.Inflate(0, -1);
        e.Graphics.DrawRectangle(Pens.DarkGray, rect);
        e.DrawFocusRectangle();
    }
}
