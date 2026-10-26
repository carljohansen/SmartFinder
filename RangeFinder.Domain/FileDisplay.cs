using System.Data;
using System.Drawing;

namespace RangeFinder.Domain;

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

    public FileAttributes Attributes { get; private set; }

    public string DisplayLocation => Location.LocationDisplay ?? Location.Location;

    public string? LocationTooltip => Location.LocationDisplay != null ? Location.Location : null;

    public bool IsFolder() => Attributes.HasFlag(FileAttributes.Directory);

    public bool Exists()
    {
        return IsFolder() ? Directory.Exists(FullPath) : File.Exists(FullPath);
    }

    public string GetDirectoryName()
    {
        return IsFolder()
                ? FullPath
                : Path.GetDirectoryName(FullPath) ?? string.Empty;
    }

    public FileDisplay(string relativePath, FileLocationRepository locationRepo)
    {
        FullPath = Path.GetFullPath(Path.Combine(locationRepo.BaseDirectory, relativePath));
        Name = Path.GetFileName(FullPath);
        Location = locationRepo.Get(FullPath);
        SizeDisplay = "";
    }

    public static FileDisplay CreateFolder(string directoryPath, string baseDirectory, FileLocationRepository locationRepo)
    {
        var matchingDirFullPath = Path.GetFullPath(Path.Combine(baseDirectory, directoryPath));

        var folderMetadata = ExtensionMetadataRepository.Instance.GetFolderMetadata();

        var result = new FileDisplay(Path.GetFileName(directoryPath), locationRepo)
        {
            FullPath = matchingDirFullPath,
            Attributes = FileAttributes.Directory,
            FileTypeDescription = folderMetadata.FriendlyDocName,
            Location = new FileLocation(Path.GetDirectoryName(matchingDirFullPath)!, 50, baseDirectory),
            Icon = folderMetadata.Icon
        };

        return result;
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

        Attributes = fileInfo.Attributes;

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
        return Path.GetExtension(FullPath);
    }

    public void AddToDataTable(DataTable dataTable)
    {
        dataTable.Rows.Add(Icon, Name, new FileSizeDisplayComparable(Size, SizeDisplay), new LocationDisplayComparable(this), FileTypeDescription, LastModified, this);
    }
}
