namespace SharpExplorer.Domain;

public readonly struct FileDisplay
{
    public string RelativePath { get; init; }

    public string Name { get; init; }

    public string Location
    {
        get; init;
    }
    public FileDisplay(string relativePath, string baseDirectory)
    {
        RelativePath = relativePath;
        var fullName = Path.Combine(baseDirectory, relativePath);
        Name = Path.GetFileName(fullName);
        Location = Path.GetDirectoryName(fullName) ?? string.Empty;
    }
}
