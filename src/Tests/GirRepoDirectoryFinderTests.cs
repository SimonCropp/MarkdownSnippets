using System.IO;
using MarkdownSnippets;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

[UsesVerify]
public class GirRepoDirectoryFinderTests
{
    [Fact]
    public void CanFindGirRepoDir()
    {
        var path = GitRepoDirectoryFinder.FindForFilePath();
        Assert.True(Directory.Exists(path));
    }
}