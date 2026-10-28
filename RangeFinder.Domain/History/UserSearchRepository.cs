using RangeFinder.Domain.TextSearch;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RangeFinder.Domain.History;

public static class UserSearchRepository
{
    public static string BaseDirHistoryFileName => GetDtoFileName("baseDirHistory");

    public static string ExclusionsHistoryFileName => GetDtoFileName("exclusionsHistory");

    public static string InclusionsHistoryFileName => GetDtoFileName("inclusionsHistory");

    public static string SearchTextHistoryFileName => GetDtoFileName("searchTextHistory");

    public static readonly JsonSerializerOptions SerializationOptions = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        IgnoreReadOnlyProperties = true
    };

    public static string GetStorageDirectory()
    {
        // Get the LocalApplicationData folder path
        var localDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var storageDir = Path.Combine(localDataDir, "RangeFinder");
        if (!Directory.Exists(storageDir))
        {
            Directory.CreateDirectory(storageDir);
        }
        return storageDir;
    }

    public static string GetDtoFileName(string name)
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

    private static string GetFileName(int id) => $"tab_{id:000}";

    public static void SaveSearchDto(UserSearchDto dto)
    {
        SaveSearchDto(dto, GetFileName(dto.Id));
    }

    public static void DeleteSearchDto(int index)
    {
        var fileName = GetDtoFileName(GetFileName(index));
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
    }

    public static UserSearchDto[] LoadSavedSearches()
    {
        var storageDir = GetStorageDirectory();
        var searchFiles = Directory.GetFiles(storageDir, "tab_*.json");
        var savedSearches = new List<UserSearchDto>();
        foreach (var fileName in searchFiles.OrderBy(f => f))
        {
            var dto = GetSearchDto(fileName);
            if (dto != null)
            {
                savedSearches.Add(dto);
            }
        }
        return [.. savedSearches];
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

    public static void DeleteHistoryFiles()
    {
        var historyFiles = new[]
        {
            BaseDirHistoryFileName,
            ExclusionsHistoryFileName,
            InclusionsHistoryFileName,
            SearchTextHistoryFileName,
            GetDtoFileName("last")
        };

        foreach (var file in historyFiles)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}