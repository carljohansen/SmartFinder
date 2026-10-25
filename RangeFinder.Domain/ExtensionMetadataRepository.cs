using RangeFinder.Domain.FileType;
using System.Drawing;

namespace RangeFinder.Domain;

public class ExtensionMetadataRepository
{
    public readonly static ExtensionMetadataRepository Instance = new();

    private readonly Dictionary<string, ExtensionMetadata> _cache = new();

    private ExtensionMetadata? _folderMetadata = null;

    private ExtensionMetadataRepository()
    {
    }

    public ExtensionMetadata GetExtensionMetadata(string fullPath)
    {
        var extension = Path.GetExtension(fullPath).ToLowerInvariant();

        if (_cache.TryGetValue(extension, out var metadata))
        {
            return metadata;
        }

        var icon = FileIconRepository.GetSmallIcon(fullPath); // TODO: default icon for those rarae cases where we get null here (e.g. fun with RTL chars in the extension).
        var friendlyDocName = extension.Length == 0 ? "File" : FileExtensionHelper.GetFileExtensionInfo(AssocStr.FriendlyDocName, extension);
        metadata = new ExtensionMetadata(extension, friendlyDocName, icon);
        _cache[extension] = metadata;
        return metadata;
    }

    public ExtensionMetadata GetFolderMetadata()
    {
        if (_folderMetadata == null)
        {
            var icon = FileIconRepository.GetSmallIcon(Environment.CurrentDirectory);
            _folderMetadata = new ExtensionMetadata("", "File folder", icon);
        }

        return _folderMetadata!;
    }
}

public record ExtensionMetadata(string Extension, string FriendlyDocName, Icon Icon);
