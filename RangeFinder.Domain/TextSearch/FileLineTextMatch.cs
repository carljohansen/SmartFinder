using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RangeFinder.Domain.Tests")]

namespace RangeFinder.Domain.TextSearch;

public record FileLineTextMatch(string Line, int LineNumber, int StartPos, int Length, AtomicTextMatch Source) : IFileLineDisplay
{
    public int SecondarySortKey => Source.Index;

    public MatchSegmentDisplay[] GetSegments()
    {
        var (optimizedLine, optimizedStartPos) = GetOptimizedLine(200);
        var segments = new List<MatchSegmentDisplay>();
        if (StartPos > 0)
        {
            segments.Add(new(optimizedLine[..optimizedStartPos], 0));
        }
        segments.Add(new(optimizedLine[optimizedStartPos..(optimizedStartPos + Length)], Source.Index + AtomicTextMatch.MatchHighlightColourIndexBase - 1));
        if (StartPos + Length < Line.Length)
        {
            segments.Add(new(optimizedLine[(optimizedStartPos + Length)..], 0));
        }
        return [.. segments];
    }

    internal (string optimizedLine, int optimizedStartPos) GetOptimizedLine(int maxLineLength)
    {
        var numLeadingSpaces = Source.HasLeadingWhitespace ? 0 : CountLeadingSpaces(); // Don't fool around with trimming in the unusual case where they are searching for leading whitespace.
        var trimmedLineLength = Line.Length - numLeadingSpaces;
        // var trimmedStartPos = StartPos - numLeadingSpaces;

        if (trimmedLineLength <= maxLineLength)
        {
            return (Line[numLeadingSpaces..], StartPos - numLeadingSpaces);
        }

        // For longer lines, return a substring and adjusted start position
        const int maxNumCharsBefore = 20;
        int contextStringStartPos = Math.Max(numLeadingSpaces, StartPos - maxNumCharsBefore);
        var actualNumCharsBefore = StartPos - contextStringStartPos;
        var numCharsAfter = maxLineLength - actualNumCharsBefore - Length; // BUG: adjust length if search text has leading whitespace and we have trimmed the line.
        int contextStringEndPos = Math.Min(Line.Length, StartPos + Length + numCharsAfter);

        var optimizedLine = Line[contextStringStartPos..contextStringEndPos];
        var optimizedStartPos = StartPos - contextStringStartPos;

        if (contextStringStartPos > numLeadingSpaces)
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

    private int CountLeadingSpaces()
    {
        int numLeadingSpaces = 0;
        while (numLeadingSpaces < Line.Length && Line[numLeadingSpaces] == ' ')
        {
            numLeadingSpaces++;
        }
        return numLeadingSpaces;
    }
}

public record MatchSegmentDisplay(string SegmentText, int SegmentColourIndex);