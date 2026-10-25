using RangeFinder.Domain.History;
using System.Text.Json;

namespace RangeFinder.Domain;

public class DirectoryEnumerator
{
    public readonly static DirectoryEnumerator Instance = new();

    public readonly Dictionary<string, List<string>> _inaccessibleDirs;

    private DirectoryEnumerator()
    {
        _inaccessibleDirs = Load(GetFileName());
    }

    public static string NormalizePath(string directoryPath)
    {
        var result = Path.GetFullPath(directoryPath);
        return !Path.EndsInDirectorySeparator(result) ? result + Path.DirectorySeparatorChar : result;
    }

    public bool HasCachedResult(string directoryPath)
    {
        directoryPath = NormalizePath(directoryPath);

        if (_inaccessibleDirs.ContainsKey(directoryPath))
        {
            return true;
        }

        return _inaccessibleDirs.Keys.Any(path => directoryPath.StartsWith(path, StringComparison.InvariantCultureIgnoreCase));
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
                preNormalizedPaths = DirectorySearch.GetInaccessibleSubdirectories(directoryPath); // This is the slow part.

                foreach (var basePath in _inaccessibleDirs.Keys)
                {
                    if (basePath.StartsWith(directoryPath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // We supersede this previous entry.
                        _inaccessibleDirs.Remove(basePath);
                    }
                }

                _inaccessibleDirs[directoryPath] = preNormalizedPaths;
                Save(_inaccessibleDirs, GetFileName());
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

    private static string GetFileName() => UserSearchRepository.GetDtoFileName("xdirs");

    private static void Save(Dictionary<string, List<string>> dict, string filePath)
    {
        try
        {
            string json = JsonSerializer.Serialize(dict, UserSearchRepository.SerializationOptions);
            File.WriteAllText(filePath, json);
        }
        catch
        {
        }
    }

    private static Dictionary<string, List<string>> Load(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return [];
            }

            string json = File.ReadAllText(filePath);
            var dict = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);

            return dict ?? [];
        }
        catch
        {
            return [];
        }
    }
}