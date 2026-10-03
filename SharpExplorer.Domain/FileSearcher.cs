using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System.Diagnostics;

namespace SharpExplorer.Domain;

public static class FileSearcher
{
    public static PatternMatchingResult DoSearch(string directoryPath, string pattern)
    {
        try
        {
            // Create matcher and add pattern
            var matcher = new Matcher();
            matcher.AddInclude(pattern);

            matcher.AddExclude(@"\System Volume Information\**");

            // Wrap directory for globbing
            var directoryInfo = new DirectoryInfo(directoryPath);
            var directoryWrapper = new DirectoryInfoWrapper(directoryInfo);

            // Execute match
            var result = matcher.Execute(directoryWrapper);

            return result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }
}
