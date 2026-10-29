
namespace RangeFinder.Domain.TextSearch;

public record FileLinePlainPreview(string Line, int LineNumber) : IFileLineDisplay
{
    public int SecondarySortKey => 0;

    public MatchSegmentDisplay[] GetSegments()
    {
        return [new(Line, 1)];
    }
}