using System.Data;
using System.Drawing;

namespace SharpExplorer.Domain;

public class FileDisplay
{
    public string FullPath { get; init; }

    public Icon? Icon { get; set; }

    public string Name { get; init; }

    public long Size { get; private set; }

    public string SizeDisplay { get; private set; }

    public DateTime LastModified { get; private set; }

    public string FileTypeDescription { get; private set; } = string.Empty;

    public FileLocation Location { get; init; }

    public string DisplayLocation => Location.LocationDisplay ?? Location.Location;

    public string? LocationTooltip => Location.LocationDisplay != null ? Location.Location : null;

    public FileDisplay(string relativePath, string baseDirectory, FileLocationRepository locationRepo)
    {
        FullPath = Path.GetFullPath(Path.Combine(baseDirectory, relativePath));
        Name = Path.GetFileName(FullPath);
        Location = locationRepo.Get(FullPath);
        SizeDisplay = "";
    }

    public void InitializeMetadata()
    {
        if (LastModified != default)
        {
            return; // Already done.
        }

        var fullName = Path.Combine(Location.Location, Name);

        //if (File.Exists(fullPath)) Should be optimistic; catch 404 on new FileInfo
        //{
        var extensionMetadata = ExtensionMetadataRepository.Instance.GetExtensionMetadata(fullName);
        Icon = extensionMetadata.Icon;
        FileTypeDescription = extensionMetadata.FriendlyDocName ?? string.Empty;
        FileInfo fileInfo = new(FullPath);

        Size = fileInfo.Length; // File size in bytes
        SizeDisplay = BytesToString(Size);
        LastModified = fileInfo.LastWriteTime; // Last modified date/time
        //}
    }

    private static String BytesToString(long byteCount)
    {
        string[] suf = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB

        if (byteCount == 0)

            return "0 KB";

        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 0);
        return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
    }

    public string GetFileExtension()
    {
        return Path.GetExtension(Name);
    }

    public void AddToDataTable(DataTable dataTable)
    {
        dataTable.Rows.Add(Icon, Name, new FileSizeDisplayComparable(Size, SizeDisplay), DisplayLocation, FileTypeDescription, LastModified, this);
    }

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