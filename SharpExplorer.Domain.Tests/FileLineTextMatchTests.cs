using SharpExplorer.Domain.TextSearch;

namespace SharpExplorer.Domain.Tests;

public class FileLineTextMatchTests
{
    [Fact]
    public void SplitsIntoSegmentsCorrectly()
    {
        var line = "This is a sample line with the word hello.";
        var match = new FileLineTextMatch(line, 1, 10, 6, new AtomicTextMatch("sample", TextMatchingOptions.None, false, 0)); // "sample" starts at index 10 and has length 6
        var segments = match.GetSegments();
        Assert.Equal(3, segments.Length);
        Assert.Equal("This is a ", segments[0].SegmentText);
        Assert.Equal(0, segments[0].SegmentColourIndex);
        Assert.Equal("sample", segments[1].SegmentText);
        Assert.Equal(1, segments[1].SegmentColourIndex);
        Assert.Equal(" line with the word hello.", segments[2].SegmentText);
        Assert.Equal(0, segments[2].SegmentColourIndex);
    }

    [Fact]
    public void GetsOptimizedLine()
    {
        var longLine = @"You can replace the default context menu with a custom one in ContextMenuOpenning event. In the menu item Click event handler, you can get the index of the grid's current row, which will be the index of right-clicked row. This is a migrated thread and some comments may be shown as answers.";
        var match = new FileLineTextMatch(longLine, 1, 50, 6, new AtomicTextMatch("sample", TextMatchingOptions.None, false, 0));
        var (optimizedLine, optimizedStartPos) = match.GetOptimizedLine(40);

        Assert.NotNull(optimizedLine);
    }
}