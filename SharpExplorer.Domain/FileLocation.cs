using System.Diagnostics.CodeAnalysis;

namespace SharpExplorer.Domain;

public class FileLocation
{
    public required string Location { get; init; }

    public string? LocationDisplay { get; private set; }

    public int CurrentDisplayLength { get; private set; }

    [SetsRequiredMembers]
    public FileLocation(string location, int initialDisplayLength)
    {
        Location = location;
        SetDisplayLength(initialDisplayLength);
    }

    public void SetDisplayLength(int displayLength)
    {
        CurrentDisplayLength = displayLength;
        LocationDisplay = GetLocationDisplay(displayLength);
    }    

    public string? GetLocationDisplay(int maxLength) => ShortenPath(Location, maxLength);

    private static string? ShortenPath(string path, int maxLength)
    {
        if (path.Length <= maxLength)
        {
            return null;
        }

        var pathElements = DirectorySplit(new DirectoryInfo(path)).Reverse().ToArray();
        var currLength = pathElements[0].Length;
        var leftMaxIncludeIndex = 0;
        var leftIndex = 1;
        var rightMaxIncludeIndex = pathElements.Length - 1;
        var rightIndex = pathElements.Length - 2;

        while (leftIndex < rightIndex)
        {
            int nextIndex;

            if (pathElements[leftIndex].Length < pathElements[rightIndex].Length)
            {
                nextIndex = leftIndex++;
                currLength += pathElements[nextIndex].Length;
                if (currLength < maxLength)
                    leftMaxIncludeIndex = leftIndex - 1;
            }
            else
            {
                nextIndex = rightIndex--;
                currLength += pathElements[nextIndex].Length;
                if (currLength < maxLength)
                    rightMaxIncludeIndex = rightIndex + 1;
            }

            if (currLength > maxLength)
                break;
        }

        var result = pathElements[0];
        for (var i = 1; i <= leftMaxIncludeIndex; i++)
        {
            result = Path.Combine(result, pathElements[i]);
        }

        result = Path.Combine(result, "...");
        for (var i = rightMaxIncludeIndex; i < pathElements.Length; i++)
        {
            result = Path.Combine(result, pathElements[i]);
        }

        return result;
    }

    public static IEnumerable<string> DirectorySplit(DirectoryInfo directory)
    {
        while (directory != null)
        {
            yield return directory.Name;
            directory = directory.Parent!;
        }
    }
}
