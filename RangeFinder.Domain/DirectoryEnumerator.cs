namespace RangeFinder.Domain;

public class DirectoryEnumerator
{
    public readonly static DirectoryEnumerator Instance = new();

    public readonly Dictionary<string, List<string>> _inaccessibleDirs = new(StringComparer.OrdinalIgnoreCase);

    private DirectoryEnumerator()
    {
    }

    public static string NormalizePath(string directoryPath)
    {
        var result = Path.GetFullPath(directoryPath);
        return !Path.EndsInDirectorySeparator(result) ? result + Path.DirectorySeparatorChar : result;
    }

    public List<string> GetInaccessibleSubdirectories(string directoryPath)
    {
        directoryPath = NormalizePath(directoryPath);

        List<string>? preNormalizedPaths = null;
        if (_inaccessibleDirs.TryGetValue(directoryPath, out var cachedPaths))
        {
            preNormalizedPaths = cachedPaths;
        }
        else
        {
            var foundSubMatch = false;
            foreach (var basePath in _inaccessibleDirs.Keys)
            {
                if (directoryPath.StartsWith(basePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    preNormalizedPaths = _inaccessibleDirs[basePath]; // Nope: have to remove the ones that are not in the deeper path.
                    foundSubMatch = true;
                    break;
                }
            }

            if (!foundSubMatch)
            {
                preNormalizedPaths = DirectorySearch.GetInaccessibleSubdirectories(directoryPath);

                foreach (var basePath in _inaccessibleDirs.Keys)
                {
                    if (basePath.StartsWith(directoryPath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // We supersede this previous entry.
                        _inaccessibleDirs.Remove(basePath);
                    }
                }

                _inaccessibleDirs[directoryPath] = preNormalizedPaths;
            }
        }

        return RelativizePaths(directoryPath, preNormalizedPaths!);
    }

    private static List<string> RelativizePaths(string basePath, List<string> subPaths)
    {
        return [.. subPaths
                    .Select(sub => Path.GetRelativePath(basePath, sub))
                    .Where(p => !p.StartsWith(".."))];
    }
}