namespace RangeFinder.Domain.Tests;

public class DirectoryEnumeratorTests
{
    [Fact]
    public void FindsAndCachesInaccessiblePaths()
    {
        var testBase = @"C:\Users\carl\AppData\Local\Microsoft\Windows\";

        var actual = DirectoryEnumerator.Instance.GetInaccessibleSubdirectories(testBase, CancellationToken.None);

        Assert.NotEmpty(actual);

        testBase = @"c:\users\carl\appdata\local";

        var a2 = DirectoryEnumerator.Instance.GetInaccessibleSubdirectories(testBase, CancellationToken.None);
    }
}