using SharpExplorer.Domain.History;
using SharpExplorer.Domain.TextSearch;

namespace SharpExplorer.Domain.Tests;

public class AtomicTextMatchTests
{
    [Theory]
    [InlineData("sample", "This is a sample line with the word hello.", 10)]
    [InlineData("sample", "This is a Sample line with the word hello.", 10)]
    [InlineData("samplex", "This is a sample line with the word hello.", -1)]
    [InlineData("thewholething", "thewholething", 0)]
    public void Match_ReturnsCorrectIndex_WhenTextIsFound(string textPattern, string line, int expectedIndex)
    {
        var textMatch = new AtomicTextMatch(textPattern, TextMatchingOptions.WholeWord, false, 0);

        var actual = textMatch.FindInLine(line);

        Assert.Equal(expectedIndex, actual);
        if (expectedIndex >= 0)
        {
            var strAtActual = actual >= 0 ? line.Substring(actual, textPattern.Length) : "not found";
            Assert.Equal(0, string.Compare(textPattern, strAtActual, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Theory]
    [InlineData("sample", "This is a sample line with the word hello.", 10)]
    [InlineData("sample", "This is a Sample line with the word hello.", 10)]
    [InlineData("samplex", "This is a sample line with the word hello.", -1)]
    [InlineData("sample", "This is a simple line with the word samples.", -1)]
    [InlineData("thewholething", "thewholething", 0)]
    [InlineData("second", "The obvious match, seconds, doesn't work with whole word matching.  But 'second' must!", 73)]
    public void Match_ReturnsCorrectIndex_WhenTextIsFound_WithWholeWordOptions(string textPattern, string line, int expectedIndex)
    {
        var textMatch = new AtomicTextMatch(textPattern, TextMatchingOptions.WholeWord, false, 0);

        var actual = textMatch.FindInLine(line);
        Assert.Equal(expectedIndex, actual);
        if (expectedIndex >= 0)
        {
            var strAtActual = actual >= 0 ? line.Substring(actual, textPattern.Length) : "not found";
            Assert.Equal(0, string.Compare(textPattern, strAtActual, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Theory]
    [InlineData("sample", "This is a sample line with the word hello.", 10)]
    [InlineData("sample", "This is a Sample line with the word hello.", -1)]
    [InlineData("samplex", "This is a sample line with the word hello.", -1)]
    [InlineData("thewholething", "thewholething", 0)]
    public void Match_ReturnsCorrectIndex_WhenTextIsFound_CaseSensitive(string textPattern, string line, int expectedIndex)
    {
        var textMatch = new AtomicTextMatch(textPattern, TextMatchingOptions.WholeWord | TextMatchingOptions.CaseSensitive, false, 0);

        var actual = textMatch.FindInLine(line);

        Assert.Equal(expectedIndex, actual);
        if (expectedIndex >= 0)
        {
            var strAtActual = actual >= 0 ? line.Substring(actual, textPattern.Length) : "not found";
            Assert.Equal(0, string.Compare(textPattern, strAtActual, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Theory]
    [InlineData("sample", "This is a sample line with the word hello.", 10)]
    [InlineData("sample", "This is a Sample line with the word hello.", -1)]
    [InlineData("samplex", "This is a sample line with the word hello.", -1)]
    [InlineData("sample", "This is a simple line with the word samples.", -1)]
    [InlineData("thewholething", "thewholething", 0)]
    public void Match_ReturnsCorrectIndex_WhenTextIsFound_WithWholeWordOptions_CaseSensitive(string textPattern, string line, int expectedIndex)
    {
        var textMatch = new AtomicTextMatch(textPattern, TextMatchingOptions.WholeWord | TextMatchingOptions.CaseSensitive, false, 0);

        var actual = textMatch.FindInLine(line);

        Assert.Equal(expectedIndex, actual);
        if (expectedIndex >= 0)
        {
            var strAtActual = actual >= 0 ? line.Substring(actual, textPattern.Length) : "not found";
            Assert.Equal(0, string.Compare(textPattern, strAtActual, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public void SplitsRawExpressionIntoAtomicTextMatches()
    {
        var dto = new UserSearchDto
        {
            RawIncludeFilesPattern = "*.txt",
            RawExcludeFilesPattern = "",
            TextSearchPattern = "hello AND NOT world AND example",
            BaseDirectoriesPattern = "."
        };
        var atomicMatches = dto.ParseTextSearchPattern();
        Assert.Equal(3, atomicMatches.Count);
        //Assert.Contains(atomicMatches, m => m.TextPattern == "hello" && !m.IsExclusion);
        //Assert.Contains(atomicMatches, m => m.TextPattern == "world" && m.IsExclusion);
        //Assert.Contains(atomicMatches, m => m.TextPattern == "example" && !m.IsExclusion);
    }

    [Fact]
    public void FindsWholeWords()
    {
        var test = "This is Frank's place.";
        var actual = test.IndexOfWholeWord("Frank", StringComparison.Ordinal);
        Assert.Equal(-1, actual);
    }
}
