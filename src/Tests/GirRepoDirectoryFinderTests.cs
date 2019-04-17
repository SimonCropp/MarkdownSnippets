using System.Diagnostics;
using System.IO;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class GirRepoDirectoryFinderTests :
    XunitLoggingBase
{
    [Fact]
    public void CanFindGirRepoDir()
    {
        var path = GitRepoDirectoryFinder.FindForFilePath();
        Assert.True(Directory.Exists(path));
    }

    [Fact]
    public void GetHash()
    {
        var path = GitRepoDirectoryFinder.FindForFilePath();
        Trace.WriteLine(GitRepoDirectoryFinder.GetHash(path));
    }

    public GirRepoDirectoryFinderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}