using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using RangeFinder.Domain.FileType;
using System.Diagnostics;

namespace RangeFinder.Domain;

public static class FileSearcher
{
    public static PatternMatchingResult DoSearch(string directoryPath, PathCollection includePatterns, PathCollection excludePatterns)
    {
        try
        {
            // Create matcher and add pattern
            var matcher = new Matcher();
            foreach (var pattern in includePatterns.Paths)
            {
                matcher.AddInclude(pattern);
            }

            foreach (var pattern in excludePatterns.Paths)
            {
                matcher.AddExclude(pattern);
            }

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

    public static PatternMatchingResult DoNotBinarySearch(string directoryPath, PathCollection excludePatterns)
    {
        var allFiles = DoSearch(directoryPath, PathCollection.CreateForFullSearch(), excludePatterns);

        var notBinFiles = new PatternMatchingResult(allFiles.Files.Where(fpm => !FileTypeHelper.IsKnownBinaryFile(Path.GetExtension(fpm.Path))));
        return notBinFiles;
    }

    public static PatternMatchingResult FilterPaths(IEnumerable<string> directoryNames, PathCollection includePatterns, PathCollection excludePatterns)
    {
        // Create matcher and add pattern
        var matcher = new Matcher();
        foreach (var pattern in includePatterns.Paths)
        {
            matcher.AddInclude(pattern);
        }

        foreach (var pattern in excludePatterns.Paths)
        {
            matcher.AddExclude(pattern);
        }

        return matcher.Match(directoryNames);
    }
}
