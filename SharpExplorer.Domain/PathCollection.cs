namespace SharpExplorer.Domain;

public class PathCollection
{
    public IReadOnlyCollection<string> Paths => _paths;

    private readonly List<string> _paths = [];

    private PathCollection(List<string> paths)
    {
        _paths = paths;
    }
    public static PathCollection Create(string rawPaths)
    {
        var paths = rawPaths.Split([';'], StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .Where(p => !string.IsNullOrWhiteSpace(p))
                            .ToList();
        return new(paths);
    }
}