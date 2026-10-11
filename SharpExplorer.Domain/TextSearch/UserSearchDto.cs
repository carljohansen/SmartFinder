namespace SharpExplorer.Domain.TextSearch;

public class UserSearchDto
{
    public required string RawIncludeFilesPattern { get; set; }

    public required string RawExcludeFilesPattern { get; set; }

    public string? TextSearchPattern { get; set; }

    public TextMatchingOptions TextMatchingOptions { get; set; } = TextMatchingOptions.None;

    public required string BaseDirectoriesPattern { get; set; }

    public bool FilterByModifiedDate { get; set; }

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
}