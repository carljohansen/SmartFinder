using SharpExplorer.Domain.TextSearch;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpExplorer.Domain;

public static class UserSearchRepository
{
    private static readonly JsonSerializerOptions SerializationOptions = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        IgnoreReadOnlyProperties = true
    };

    private static string GetStorageDirectory()
    {
        // Get the LocalApplicationData folder path
        var localDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var storageDir = Path.Combine(localDataDir, "SmartFinder");
        if (!Directory.Exists(storageDir))
        {
            Directory.CreateDirectory(storageDir);
        }
        return storageDir;
    }

    private static string GetDtoFileName(string name)
    {
        var path = Path.ChangeExtension(Path.Combine(GetStorageDirectory(), name), "json");
        return path;
    }

    public static void SaveSearchDto(UserSearchDto dto, string name)
    {
        var fileName = GetDtoFileName(name);


        string jsonString = JsonSerializer.Serialize(dto, SerializationOptions);

        // Save JSON to file
        File.WriteAllText(fileName, jsonString);
    }

    public static void SaveLastSearchDto(UserSearchDto dto)
    {
        SaveSearchDto(dto, "last");
    }

    public static UserSearchDto? GetSearchDto(string name)
    {
        var fileName = GetDtoFileName(name);
        if (!File.Exists(fileName))
        {
            return null;
        }

        var json = File.ReadAllText(fileName);

        var result = JsonSerializer.Deserialize<UserSearchDto>(json, SerializationOptions);
        return result;
    }

    public static UserSearchDto? GetLastSearchDto() => GetSearchDto("last");

    public static UserSearchDto GetDefaultSearchDto()
    {
        return new()
        {
            TextSearchPattern = "",
            RawIncludeFilesPattern = "",
            RawExcludeFilesPattern = "",
            BaseDirectoriesPattern = Directory.GetCurrentDirectory(),
            FilterByModifiedDate = false,
            TextMatchingOptions = TextMatchingOptions.None
        };
    }
}