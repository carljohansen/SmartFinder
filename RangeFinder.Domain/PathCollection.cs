namespace RangeFinder.Domain;

public class PathCollection
{
    public IReadOnlyCollection<string> Paths => _paths;

    private readonly List<string> _paths = [];

    private PathCollection(List<string> paths)
    {
        _paths = paths;
    }

    public override string ToString()
    {
        return string.Join("; ", _paths);
    }

    public static PathCollection Create(string rawPaths, PathCollectionOptions options)
    {
        if (string.IsNullOrWhiteSpace(rawPaths)
            && options.HasFlag(PathCollectionOptions.AddGeneralWildcardIfBlank))
        {
            rawPaths = "/**/*";
        }

        var paths = rawPaths.Split([';'], StringSplitOptions.RemoveEmptyEntries)
                    .Select(Globbify)
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .ToList();
        return new(paths);
    }

    public void AddRange(IEnumerable<string> morePaths)
    {
        _paths.AddRange(morePaths);
    }

    public static PathCollection CreateForFullSearch() => Create("", PathCollectionOptions.AddGeneralWildcardIfBlank);

    public static string Globbify(string path)
    {
        path = path.Trim();
        if (path.StartsWith("**"))
        {
            return "/" + path;
        }
        else if (path.StartsWith("/**"))
        {
            return path; // Nothing to do.
        }
        else if (path.StartsWith('/'))
        {
            return path; // They don't want us to touch this.
        }
        else
        {
            return $"**/{path}";
        }
    }
}