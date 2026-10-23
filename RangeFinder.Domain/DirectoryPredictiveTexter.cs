namespace RangeFinder.Domain;

public static class DirectoryPredictiveTexter
{
    public static string[] GetPossibleContinuations(string userPath)
    {
        if (Directory.Exists(userPath) && userPath.EndsWith(Path.DirectorySeparatorChar))
        {
            return GetSubdirectories(userPath);
        }
        else
        {
            var possibleSplit = Path.GetDirectoryName(userPath);
            var userLeafDirName = Path.GetFileName(userPath);

            if (Directory.Exists(possibleSplit))
            {
                var subdirs = GetSubdirectories(possibleSplit);
                var result = new List<string>();
                var fuzzyResult = new List<string>();

                foreach (var subdir in subdirs)
                {
                    var subDirLeafName = Path.GetFileName(subdir);
                    if (subDirLeafName.StartsWith(userLeafDirName, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(subdir);
                    }
                    else if (subDirLeafName.Contains(userLeafDirName, StringComparison.OrdinalIgnoreCase))
                    {
                        fuzzyResult.Add(subdir);
                    }
                }

                return [.. result, .. fuzzyResult];
            }
        }

        return [];
    }

    private static string[] GetSubdirectories(string userPath)
    {
        try
        {
            return Directory.GetDirectories(userPath);
        }
        catch (UnauthorizedAccessException)
        {
            // TODO: handle this better.
            return [];
        }
    }
}