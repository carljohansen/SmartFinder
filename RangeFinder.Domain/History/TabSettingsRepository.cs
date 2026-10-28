using System.Text.Json;

namespace RangeFinder.Domain.History;

public static class TabSettingsRepository
{
    public static string GetDtoFileName(string name)
    {
        var path = Path.ChangeExtension(Path.Combine(UserSearchRepository.GetStorageDirectory(), name), "json");
        return path;
    }

    public static void SaveTabSettings(UserTabSettings dto, string name)
    {
        var fileName = GetDtoFileName(name);


        string jsonString = JsonSerializer.Serialize(dto, UserSearchRepository.SerializationOptions);

        File.WriteAllText(fileName, jsonString);
    }

    public static void SaveTabSettings(UserTabSettings dto)
    {
        SaveTabSettings(dto, GetFileName(dto.Id));
    }

    private static string GetFileName(int id) => $"tabpref_{id:000}";

    public static UserTabSettings[] LoadSavedTabSettings()
    {
        var storageDir = UserSearchRepository.GetStorageDirectory();
        var searchFiles = Directory.GetFiles(storageDir, "tabpref_*.json");
        var savedSearches = new List<UserTabSettings>();
        foreach (var fileName in searchFiles.OrderBy(f => f))
        {
            var dto = GetTabSettings(fileName);
            if (dto != null)
            {
                savedSearches.Add(dto);
            }
        }
        return [.. savedSearches];
    }

    public static UserTabSettings? GetTabSettings(string name)
    {
        var fileName = GetDtoFileName(name);
        if (!File.Exists(fileName))
        {
            return null;
        }

        var json = File.ReadAllText(fileName);

        var result = JsonSerializer.Deserialize<UserTabSettings>(json, UserSearchRepository.SerializationOptions);
        return result;
    }

    public static void DeleteTabSettings(int index)
    {
        var fileName = GetDtoFileName(GetFileName(index));
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
    }
}
