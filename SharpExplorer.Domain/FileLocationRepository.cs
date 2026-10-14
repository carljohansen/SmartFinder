namespace SharpExplorer.Domain;

public class FileLocationRepository
{
    private readonly Dictionary<string, FileLocation> _fileLocations = [];

    public FileLocationRepository()
    {
    }

    public FileLocation Get(string fullPath)
    {
        var directory = Path.GetDirectoryName(fullPath);

        // get from cache or create.
        if (_fileLocations.TryGetValue(directory, out var location))
        {
            return location;
        }

        var newFileLocation = new FileLocation(directory);
        newFileLocation.CurrentDisplayLength = 50;
        _fileLocations[directory] = newFileLocation;

        return newFileLocation;
    }

    public void SetDisplayLength(int length)
    {
        foreach (var location in _fileLocations.Values)
        {
            location.CurrentDisplayLength = length;
        }
    }
}