using SharpExplorer.Domain.FileType;
using System.Data;

namespace SharpExplorer.Domain;

public class FileDisplay
{
    public string FullPath { get; init; }

    public string Name { get; init; }

    public long Size { get; private set; }

    public string SizeDisplay { get; private set; }

    public DateTime LastModified { get; private set; }

    public string FileTypeDescription { get; private set; } = string.Empty;

    public string Location
    {
        get; init;
    }
    public FileDisplay(string relativePath, string baseDirectory)
    {
        FullPath = Path.GetFullPath(Path.Combine(baseDirectory, relativePath));
        Name = Path.GetFileName(FullPath);
        Location = Path.GetDirectoryName(FullPath) ?? string.Empty;
        SizeDisplay = "";
    }

    public void InitializeMetadata()
    {
        if (LastModified != default)
        {
            return; // Already done.
        }

        var fullPath = Path.Combine(Location, Name);

        if (File.Exists(fullPath))
        {
            FileInfo fileInfo = new(FullPath);

            Size = fileInfo.Length; // File size in bytes
            SizeDisplay = BytesToString(Size);
            LastModified = fileInfo.LastWriteTime; // Last modified date/time
        }

        var fileTypeDesc = FileExtensionHelper.GetFileExtensionInfo(AssocStr.FriendlyDocName, GetFileExtension());
        FileTypeDescription = fileTypeDesc ?? string.Empty;
    }
    
    private static String BytesToString(long byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        if (byteCount == 0)
            return "0" + suf[0];
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
        dataTable.Rows.Add(Name, new FileSizeDisplayComparable(Size, SizeDisplay), Location, FileTypeDescription, LastModified, this);
    }
}