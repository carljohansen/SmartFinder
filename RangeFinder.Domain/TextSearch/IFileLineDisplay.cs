
namespace RangeFinder.Domain.TextSearch;

public interface IFileLineDisplay
{
    public int LineNumber { get; }

    public int SecondarySortKey { get; }

    MatchSegmentDisplay[] GetSegments();
}
