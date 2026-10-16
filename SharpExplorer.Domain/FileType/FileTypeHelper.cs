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
        ".exe", ".dll", ".bin", ".dat", ".jpg", ".jpeg", ".jfif", ".png", ".gif", ".bmp",
        ".au", ".mp3", ".wav", ".mpg", ".mpeg", ".mp4", ".m4v", ".avi", ".avif", ".mov", ".zip", ".rar", ".wmv", ".db", ".flv", ".info", ".rm",
        ".ram", ".swf", ".webp"
        // Add more known binary file extensions as needed
    ];

    public static bool IsKnownTextFile(string? fileExtension)
    {
        return fileExtension == null ? false : KnownTextFileExtensions.Contains(fileExtension.ToLower());
    }

    public static bool IsKnownBinaryFile(string fileExtension)
    {
        return KnownBinaryFileExtensions.Contains(fileExtension.ToLower());
    }
}