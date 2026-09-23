public class DownloaderTests
{
    [Test]
    public async Task Valid()
    {
        var content = await Downloader.DownloadContent("https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/.gitattributes");
        await Verify(new {content.success, content.content});
    }

    [Test]
    public async Task Missing()
    {
        var content = await Downloader.DownloadContent("https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/missing.txt");
        await Assert.That(content.success).IsFalse();
        await Assert.That(content.content).IsNull();
    }
}