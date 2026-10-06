using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SharpExplorer.Domain.Tests")]

namespace SharpExplorer.Domain.TextSearch;

public record FileLineTextMatch(string Line, int LineNumber, int StartPos, int Length, AtomicTextMatch Source)
{
    internal (string optimizedLine, int optimizedStartPos) GetOptimizedLine(int maxLineLength)
    {
        if (Line.Length <= maxLineLength)
        {
            return (Line, StartPos);
        }

        // For longer lines, return a substring and adjusted start position
        const int numCharsBefore = 20;
        var numCharsAfter = maxLineLength - numCharsBefore - Length;
        int contextStringStartPos = Math.Max(0, StartPos - numCharsBefore);
        int contextStringEndPos = Math.Min(Line.Length, StartPos + Length + numCharsAfter);

        var optimizedLine = Line[contextStringStartPos..contextStringEndPos];
        var optimizedStartPos = StartPos - contextStringStartPos;

        if (contextStringStartPos > 0)
        {
            optimizedLine = "..." + optimizedLine;
            optimizedStartPos += 3; // Adjust for the added ellipsis
        }
        if (contextStringEndPos < Line.Length)
        {
            optimizedLine += "...";
        }
        return (optimizedLine, optimizedStartPos);
    }

    public MatchSegmentDisplay[] GetSegments()
    {
        var (optimizedLine, optimizedStartPos) = GetOptimizedLine(200);
        var segments = new List<MatchSegmentDisplay>();
        if (StartPos > 0)
        {
            segments.Add(new(optimizedLine[..optimizedStartPos], 0));
        }
        segments.Add(new(optimizedLine[optimizedStartPos..(optimizedStartPos + Length)], 1));
        if (StartPos + Length < Line.Length)
        {
            segments.Add(new(optimizedLine[(optimizedStartPos + Length)..], 0));
        }
        return [.. segments];
    }
}

public record MatchSegmentDisplay(string SegmentText, int SegmentColourIndex);