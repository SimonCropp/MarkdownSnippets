public class GirRepoDirectoryFinderTests
{
    [Test]
    public async Task CanFindGirRepoDir()
    {
        var path = GitRepoDirectoryFinder.FindForFilePath();
        await Assert.That(Directory.Exists(path)).IsTrue();
    }
}