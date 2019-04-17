using System.IO;
using ApprovalTests;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class GitHashReaderTests :
    XunitLoggingBase
{
    [Fact]
    public void GetHash()
    {
        var path = GitRepoDirectoryFinder.FindForFilePath();
        var hash = GitHashReader.GetHash(path);
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void NoRef()
    {
        Verify("GitDirs/NoRef");
    }

    [Fact]
    public void WithRef()
    {
        Verify("GitDirs/WithRef");
    }

    static void Verify(string path)
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, path);
        var hash = GitHashReader.GetHashForGitDirectory(directory);
        Approvals.Verify(hash);
    }

    public GitHashReaderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}