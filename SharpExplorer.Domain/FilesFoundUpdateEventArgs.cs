namespace RangeFinder.Domain;

public class FilesFoundUpdateEventArgs : EventArgs
{
    public int FilesFound { get; }

    public FileSearchStatus SearchStatus { get; }

    public FilesFoundUpdateEventArgs(int filesFound, FileSearchStatus searchStatus)
    {
        FilesFound = filesFound;
        SearchStatus = searchStatus;
    }
}