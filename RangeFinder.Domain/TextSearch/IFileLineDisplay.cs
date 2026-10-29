
namespace RangeFinder.Domain.TextSearch;

public interface IFileLineDisplay
{
    public int LineNumber { get; }

    MatchSegmentDisplay[] GetSegments();
}
