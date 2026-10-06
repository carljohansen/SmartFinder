namespace SharpExplorer.Domain.TextSearch;

internal static class StringExtensions
{
    extension(string str)
    {
        public int IndexOfWholeWord(string word, StringComparison comparisonType)
        {
            for (int j = 0; j < str.Length
                            && (j = str.IndexOf(word, j, comparisonType)) >= 0; j++)
            {
                if ((j == 0 || !char.IsLetterOrDigit(str, j - 1))
                    && (j + word.Length == str.Length || !char.IsLetterOrDigit(str, j + word.Length)))
                {
                    // Special case: apostrophes.
                    var charIndexAfter = j + word.Length;
                    if (charIndexAfter < str.Length - 1 && str[charIndexAfter] == '\'' && char.IsLetter(str, charIndexAfter + 1))
                    {
                        continue;
                    }
                    return j;
                }
            }
            return -1;
        }
    }
}