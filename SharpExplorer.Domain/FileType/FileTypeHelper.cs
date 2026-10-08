namespace SharpExplorer.Domain.FileType;

public record FileTypeDetails(string Description, bool IsText);

public static class FileTypeHelper
{
    public readonly static string[] KnownTextFileExtensions =
    [
        ".txt", ".md", ".csv", ".json", ".xml", ".html", ".htm", ".css", ".js", ".ts",
        ".py", ".java", ".c", ".cpp", ".h", ".hpp", ".cs", ".vb", ".php", ".rb", ".yml",".yaml",
        ".ini", ".log", ".bat", ".sh", ".ps1", ".r", ".pl", ".swift", ".go",
        // Add more known text file extensions as needed
    ];

    public readonly static string[] KnownBinaryFileExtensions =
    [
        ".exe", ".dll", ".bin", ".dat", ".jpg", ".jpeg", ".png", ".gif", ".bmp",
        ".mp3", ".wav", ".mp4", ".avi", ".mov", ".zip", ".rar",
        // Add more known binary file extensions as needed
    ];

    public static bool IsKnownTextFile(string fileExtension)
    {
        return KnownTextFileExtensions.Contains(fileExtension.ToLower());
    }

    public static bool IsKnownBinaryFile(string fileExtension)
    {
        return KnownBinaryFileExtensions.Contains(fileExtension.ToLower());
    }
}