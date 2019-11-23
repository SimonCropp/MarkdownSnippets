using System.IO;
using MarkdownSnippets;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class GirRepoDirectoryFinderTests :
    VerifyBase
{
    [Fact]
    public void CanFindGirRepoDir()
    {
        var path = GitRepoDirectoryFinder.FindForFilePath();
        Assert.True(Directory.Exists(path));
    }

    public GirRepoDirectoryFinderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}