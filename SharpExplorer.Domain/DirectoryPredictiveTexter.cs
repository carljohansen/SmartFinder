namespace SharpExplorer.Domain;

public static class DirectoryPredictiveTexter
{
    public static string[] GetPossibleContinuations(string userPath)
    {
        if (Directory.Exists(userPath))
        {
            return GetSubdirectories(userPath);
        }
        else
        {
            var possibleSplit = Path.GetDirectoryName(userPath);
            var split2 = Path.GetFileName(userPath);

            if (Directory.Exists(possibleSplit))
            {
                var subdirs = GetSubdirectories(possibleSplit);
                var result = new List<string>();

                foreach (var subdir in subdirs)
                {
                    var terminalName = Path.GetFileName(subdir);
                    if (terminalName.StartsWith(split2, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(subdir);
                    }
                }

                return [.. result];
            }
        }

        return [];
    }

    private static string[] GetSubdirectories(string userPath)
    {
        return Directory.GetDirectories(userPath);
    }
}
