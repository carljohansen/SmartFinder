namespace RangeFinder.Domain;

public class FileSizeDisplayComparable : IComparable
{
    private readonly long _fileSize;
    private readonly string _fileSizeDisplay;

    public override string ToString()
    {
        return _fileSizeDisplay;
    }

    public int CompareTo(object? obj)
    {
        if (obj is FileSizeDisplayComparable other)
        {
            return _fileSize.CompareTo(other._fileSize);
        }
        throw new ArgumentException("Object is not a FileSizeDisplayComparable");
    }

    public FileSizeDisplayComparable(long fileSize, string fileSizeDisplay)
    {
        _fileSize = fileSize;
        _fileSizeDisplay = fileSizeDisplay;
    }
}