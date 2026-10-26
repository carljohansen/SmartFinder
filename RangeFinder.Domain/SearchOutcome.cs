namespace RangeFinder.Domain;

public record SearchOutcome(string StatusMessage, int? NumFilesFound, TimeSpan? ElapsedTime, FileSearchStatus FinalState)
{
    public static string GetElapsedTimeDisplay(TimeSpan? elapsed)
    {
        if (elapsed == null)
        {
            return "";
        }

        if (elapsed.Value.TotalSeconds < 1)
        {
            return $"{elapsed.Value.TotalMilliseconds:#,##0} ms";
        }
        else if (elapsed.Value.TotalMinutes < 2)
        {
            return $"{elapsed.Value.TotalSeconds:#,##0} seconds";
        }
        else if (elapsed.Value.TotalHours < 1)
        {
            return $"{elapsed.Value.TotalMinutes:#,##0} minutes";
        }
        else
        {
            return $"{elapsed.Value.TotalHours:#,##0} hours";
        }
    }

    public string GetElapsedTimeDisplay()
    {
        return GetElapsedTimeDisplay(ElapsedTime);
    }

    public string GetFilesFoundDisplay()
    {
        if (NumFilesFound == null)
        {
            return "";
        }
        if (NumFilesFound == 0)
        {
            return "No matching files found";
        }
        return NumFilesFound == 1 ? "1 file" : $"{NumFilesFound:#,##0} files";
    }
}
