namespace SharpExplorer.Domain;

public class LocationDisplayComparable : IComparable
{
    private FileLocation _fileLocation;

    public override string ToString()
    {
        return _fileLocation.LocationDisplay ?? _fileLocation.Location;
    }

    public int CompareTo(object? obj)
    {
        if (obj is LocationDisplayComparable other)
        {
            return _fileLocation.Location.CompareTo(other._fileLocation.Location);
        }
        throw new ArgumentException("Object is not a LocationDisplayComparable");
    }

    public LocationDisplayComparable(FileLocation fileLocation)
    {
        _fileLocation = fileLocation;
    }

    public LocationDisplayComparable(FileDisplay fileDisplay) : this(fileDisplay.Location)
    {
    }
}