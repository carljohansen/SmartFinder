using System.Text.RegularExpressions;

namespace RangeFinder.Domain;

public record DirectorySearchResult(List<string> DirectoryNames, List<string> ErrorDirectoryNames);

public static partial class DirectorySearch
{
    public static DirectorySearchResult GetAllSubdirectories(string rootPath)
    {
        var result = new DirectorySearchResult([], []);
        GetAllSubdirectories(rootPath, result, findInaccessibleOnly: false);
        return result;
    }

    public static List<string> GetInaccessibleExclusionPaths(string rootPath)
    {
        var inaccessibleDirectoryNames = GetInaccessibleSubdirectories(rootPath);
        for (var i = 0; i < inaccessibleDirectoryNames.Count; i++)
        {
            inaccessibleDirectoryNames[i] = Path.GetRelativePath(rootPath, inaccessibleDirectoryNames[i]);
        }
        return inaccessibleDirectoryNames;
    }

    public static List<string> GetInaccessibleSubdirectories(string rootPath)
    {
        var result = new DirectorySearchResult([], []);
        GetAllSubdirectories(rootPath, result, findInaccessibleOnly: true);
        return result.ErrorDirectoryNames;
    }

    /// <summary>
    /// Recursively retrieves all subdirectories, skipping those with denied access.
    /// </summary>
    /// <param name="rootPath">The starting directory path.</param>
    /// <returns>A list of all accessible subdirectories.</returns>
    private static void GetAllSubdirectories(string rootPath, DirectorySearchResult result, bool findInaccessibleOnly)
    {
        try
        {
            // Get immediate subdirectories
            string[] subDirs = Directory.GetDirectories(rootPath);

            if (!findInaccessibleOnly)
            {
                result.DirectoryNames.AddRange(subDirs);
            }

            foreach (string dir in subDirs)
            {
                // Recursively get subdirectories of this directory
                GetAllSubdirectories(dir, result, findInaccessibleOnly);
            }
        }
        catch (Exception ex)
        {
            var dirName = ExtractDirName(ex.Message);
            if (dirName != null)
            {
                result.ErrorDirectoryNames.Add(dirName);
            }
            // Skip directories we cannot access
        }
    }

    private static string? ExtractDirName(string exMessage)
    {
        var match = PathInErrorMessageRegex().Match(exMessage);
        if (!match.Success)
        {
            return null;
        }
        return match.Groups[1].Value;
    }

    [GeneratedRegex(@"'(.*)'")]
    private static partial Regex PathInErrorMessageRegex();
}
