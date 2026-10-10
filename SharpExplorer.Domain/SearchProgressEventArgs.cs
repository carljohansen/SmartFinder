namespace SharpExplorer.Domain;

public class SearchProgressEventArgs : EventArgs
{
    public required string StatusMessage { get; set; }
}
