namespace RangeFinder.Domain;

public class SearchProgressEventArgs : EventArgs
{
    public required string StatusMessage { get; set; }

    public required int ProgressPercentage { get; set; }
}


public class ExtractionProgressEventArgs : EventArgs
{
    public required int ProgressPercentage { get; set; }
}
