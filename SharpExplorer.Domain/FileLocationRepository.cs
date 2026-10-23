namespace RangeFinder.Domain;

public class FileLocationRepository
{
    private readonly Dictionary<string, FileLocation> _fileLocations = [];

    public int CurrentDisplayLength { get; private set; }

    public string BaseDirectory { get; private set; }

    public FileLocationRepository(int initialDisplayLength)
    {
        CurrentDisplayLength = initialDisplayLength;
        BaseDirectory = "";
    }

    public void Start(string baseDirectory)
    {
        BaseDirectory = baseDirectory;
        _fileLocations.Clear();
    }

    public FileLocation Get(string fullPath)
    {
        var directory = Path.GetDirectoryName(fullPath) ?? "";

        // get from cache or create.
        if (_fileLocations.TryGetValue(directory, out var location))
        {
            return location;
        }

        var newFileLocation = new FileLocation(directory, CurrentDisplayLength, BaseDirectory);
        _fileLocations[directory] = newFileLocation;

        return newFileLocation;
    }

    public void SetDisplayLength(int displayLength)
    {
        CurrentDisplayLength = displayLength;
        foreach (var location in _fileLocations.Values)
        {
            location.SetDisplayLength(displayLength);
        }
    }
}