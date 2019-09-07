using System.IO;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class GirRepoDirectoryFinderTests :
    XunitApprovalBase
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