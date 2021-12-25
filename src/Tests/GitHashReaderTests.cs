using MarkdownSnippets;

[UsesVerify]
public class GitHashReaderTests
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
    public Task NoRef()
    {
        return VerifyInner("GitDirs/NoRef");
    }

    [Fact]
    public Task WithRef()
    {
        return VerifyInner("GitDirs/WithRef");
    }

    static Task VerifyInner(string path)
    {
        var directory = Path.Combine(AttributeReader.GetProjectDirectory(), path);
        var hash = GitHashReader.GetHashForGitDirectory(directory);
        return Verify(hash);
    }
}