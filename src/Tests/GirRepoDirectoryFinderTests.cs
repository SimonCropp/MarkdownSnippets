using MarkdownSnippets;

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