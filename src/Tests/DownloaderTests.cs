public class DownloaderTests
{
    [Test]
    public async Task Valid()
    {
        var content = await Downloader.DownloadContent("https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/.gitattributes");
        await Verify(new {content.success, content.content})
            .Snapshot(
                """
                {
                  success: true,
                  content:
                # Auto detect text files and normalize line endings to LF
                * text=auto eol=lf
                *.png binary
                *.snk binary

                *.verified.txt text eol=lf working-tree-encoding=UTF-8
                *.verified.md text eol=lf working-tree-encoding=UTF-8
                *.verified.mdx text eol=lf working-tree-encoding=UTF-8
                *.verified.xml text eol=lf working-tree-encoding=UTF-8
                *.verified.json text eol=lf working-tree-encoding=UTF-8

                .editorconfig text eol=lf working-tree-encoding=UTF-8
                *.sln.DotSettings text eol=lf working-tree-encoding=UTF-8
                *.slnx.DotSettings text eol=lf working-tree-encoding=UTF-8

                }
                """);
    }

    [Test]
    public async Task Missing()
    {
        var content = await Downloader.DownloadContent("https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/missing.txt");
        await Assert.That(content.success).IsFalse();
        await Assert.That(content.content).IsNull();
    }
}