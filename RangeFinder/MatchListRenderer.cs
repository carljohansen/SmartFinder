using RangeFinder.Controls;
using RangeFinder.Domain.TextSearch;

namespace RangeFinder;

internal class MatchListRenderer
{
    private Font? _highlightFont;

    private readonly Brush[] _brushes = [.. new[] { Color.Black, Color.Orange, Color.Purple, Color.CornflowerBlue, Color.DarkKhaki, Color.Green }.Select(c => new SolidBrush(c))];

    public readonly static MatchListRenderer Instance = new();

    private MatchListRenderer()
    {
    }

    private Font GetHighlightFont(ListView listView)
    {
        Font font = listView.Font;
        _highlightFont ??= new Font(font.FontFamily, font.Size, FontStyle.Bold, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
        return _highlightFont;
    }

    public void DrawMatchListViewSubItem(FileLineTextMatch? itemUnderlyingMatches, ListView listView, DrawListViewSubItemEventArgs e)
    {
        if (e.ItemIndex < 0)
            return;

        Font font = listView.Font;
        var highlightFont = GetHighlightFont(listView);

        using Graphics g = e.Graphics;

        if (e.ColumnIndex == 0)
        {
            // Line Number:
            string lineNumberText = e.SubItem!.Text;
            Brush brush = _brushes[0];
            g.DrawString(lineNumberText, highlightFont, brush, e.Bounds.Left + 5, e.Bounds.Top + 3, StringFormat.GenericTypographic);
        }
        else
        {
            if (itemUnderlyingMatches == null)
            {
                return;
            }

            var textSegments = itemUnderlyingMatches.GetSegments();
            float xPos = e.Bounds.Left;

            // Text:
            foreach (var segment in textSegments)
            {
                string segmentText = segment.SegmentText;
                Brush brush = _brushes[Math.Min(segment.SegmentColourIndex, _brushes.Length - 1)];
                var segmentFont = segment.SegmentColourIndex == 0 ? font : highlightFont;
                g.DrawString(segmentText, segmentFont, brush, xPos, e.Bounds.Top + 3, StringFormat.GenericTypographic);
                xPos += g.MeasureString(segmentText, segmentFont, 0, MeasuringHelper.FormatForMeasuring).Width;
            }
        }
    }
}