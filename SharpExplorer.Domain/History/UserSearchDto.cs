using RangeFinder.Domain.TextSearch;

namespace RangeFinder.Domain.History;

public class UserSearchDto
{
    public static string NormalizeDirectoryName(string directoryName)
    {
        var result = directoryName;

        if (directoryName.Length > 2 && directoryName.StartsWith('\"') && directoryName.EndsWith('\"'))
        {
            result = directoryName[1..^1];
        }

        if (result.Length > 2 && result.EndsWith('\\'))
        {
            result = result[..^1];
        }

        return result;
    }

    public required string? RawIncludeFilesPattern { get; set; }

    public required string? RawExcludeFilesPattern { get; set; }

    public string GetExcludeFilesPattern()
    {
        return EnableFileExclusion ? RawExcludeFilesPattern ?? "" : "";
    }

    public int Id { get; set; }

    public string? TextSearchPattern { get; set; }

    public TextMatchingOptions TextMatchingOptions { get; set; } = TextMatchingOptions.None;

    public required string BaseDirectoriesPattern { get; set; }

    public bool EnableFileExclusion { get; set; }

    public bool FilterByModifiedDate { get; set; }

    public bool ShowHiddenItems { get; set; }

    public float? ModifiedInLastQuantity { get; set; }

    public string? ModifiedInLastQuantityType { get; set; }

    public DateTimeOffset? ModifiedBefore { get; set; }

    public DateTimeOffset? ModifiedAfter
    {
        get
        {
            if (!FilterByModifiedDate
                || ModifiedInLastQuantity == null)
            {
                return null;
            }

            var dateFilterQty = ModifiedInLastQuantity.Value * -1;

            var dateFilterQtyType = ModifiedInLastQuantityType;
            if (string.IsNullOrEmpty(dateFilterQtyType))
            {
                return null;
            }

            var referenceTime = DateTimeOffset.UtcNow;
            var result = dateFilterQtyType switch
            {
                "hours" => referenceTime.AddHours(dateFilterQty),
                "days" => referenceTime.AddDays(dateFilterQty),
                "weeks" => referenceTime.AddDays(dateFilterQty * 7),
                "months" => referenceTime.AddMonths((int)dateFilterQty),
                "years" => referenceTime.AddYears((int)dateFilterQty),
                _ => throw new InvalidOperationException()
            };

            return result;
        }
    }

    public bool HasMetadataCriteria => ModifiedAfter.HasValue;

    public bool HasTextSearch() => !string.IsNullOrWhiteSpace(TextSearchPattern);

    public IReadOnlyCollection<AtomicTextMatch> ParseTextSearchPattern()
    {
        if (string.IsNullOrWhiteSpace(TextSearchPattern))
        {
            return [];
        }

        string[] parts = TextSearchPattern.Split([" AND "], StringSplitOptions.RemoveEmptyEntries);

        var result = new List<AtomicTextMatch>();
        var index = 1;
        foreach (string part in parts)
        {
            var isExclusion = false;
            string? exclusionPart = null;
            if (part.StartsWith("NOT ", StringComparison.Ordinal))
            {
                isExclusion = true;
                exclusionPart = part[4..]; // Remove the "NOT " prefix
            }
            var atomicMatch = new AtomicTextMatch(exclusionPart ?? part, TextMatchingOptions, isExclusion, isExclusion ? 0 : index++);
            result.Add(atomicMatch);
        }

        return result;
    }

    public static UserSearchDto CreateDefault() => new()
    {
        Id = 0,
        BaseDirectoriesPattern = "",
        RawIncludeFilesPattern = "",
        RawExcludeFilesPattern = "",
        EnableFileExclusion = false,
        ShowHiddenItems = false,
        FilterByModifiedDate = false
    };

    public string? AddFolderExclusion(string folderPath)
    {
        var folderRelativePath = GetRelativePath(folderPath);
        var currentExclusions = this.RawExcludeFilesPattern;
        if (folderRelativePath == null)
        {
            return currentExclusions;
        }
        if (string.IsNullOrWhiteSpace(currentExclusions))
        {
            return folderRelativePath;
        }
        if (currentExclusions.Contains(folderRelativePath))
        {
            return currentExclusions;
        }
        else
        {
            return $"{currentExclusions};{folderRelativePath}";
        }
    }

    public string? GetRelativePath(string folderPath)
    {
        var baseDir = Path.GetFullPath(BaseDirectoriesPattern);
        var relativeFolder = Path.GetRelativePath(baseDir, folderPath);

        if (relativeFolder == ".")
        {
            // Excluding the base dir itself is not allowed (would guarantee no results).
            // To exclude the base dir, use the _inclusion_ pattern /*/**
            return null;
        }

        var normalized = relativeFolder.Replace(Path.DirectorySeparatorChar, '/');
        if (!normalized.EndsWith('/'))
        {
            normalized += "/";
        }
        return normalized;
    }

    public string GetDisplayTitle()
    {
        const int maxLength = 15;

        var unelidedResult = "New Search";

        if (!string.IsNullOrWhiteSpace(BaseDirectoriesPattern)
            && !string.IsNullOrWhiteSpace(RawIncludeFilesPattern))
        {
            var tryCombine = $"{BaseDirectoriesPattern} ({RawIncludeFilesPattern})";
            if (tryCombine.Length <= maxLength)
            {
                // Unlikely, but it could happen.
                return tryCombine;
            }
        }

        if (!string.IsNullOrWhiteSpace(TextSearchPattern))
        {
            var elidedSearchText = Elide(TextSearchPattern, maxLength - 2);
            return $"\"{elidedSearchText}\"";
        }
        else if (!string.IsNullOrWhiteSpace(BaseDirectoriesPattern))
        {
            unelidedResult = BaseDirectoriesPattern;
        }
        else if (!string.IsNullOrWhiteSpace(RawIncludeFilesPattern))
        {
            unelidedResult = RawIncludeFilesPattern;
        }

        if (unelidedResult.Length <= maxLength)
        {
            return unelidedResult;
        }

        return Elide(unelidedResult, maxLength);
    }

    private static string Elide(string str, int maxLength)
    {
        const string ellipsis = "...";

        if (str.Length <= maxLength)
        {
            return str;
        }

        var visibleLength = Math.Max(0, maxLength - ellipsis.Length);
        return str[..visibleLength] + ellipsis;
    }
}