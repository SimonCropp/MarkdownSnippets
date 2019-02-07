using System.IO;
using MarkdownSnippets;
using Xunit;

public class GirRepoDirectoryFinderTests
{
    [Fact]
    public void CanFindGirRepoDir()
    {
        var path = GitRepoDirectoryFinder.FindForFilePath();
        Assert.True(Directory.Exists(path));
    }
}