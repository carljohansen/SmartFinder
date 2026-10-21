namespace SharpExplorer.Domain;

public class FileSearchOptions
{
    /// <summary>
    /// Gets or sets a flag that indicates whether to exclude non-text files from text searches when no include patterns are specified.
    /// </summary>
    public bool AutomaticTextSearchFileFiltering { get; set; }
}