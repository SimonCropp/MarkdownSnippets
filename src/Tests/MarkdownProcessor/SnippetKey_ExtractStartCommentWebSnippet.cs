public class SnippetKey_ExtractStartCommentWebSnippet
{
    [Test]
    public async Task Simple()
    {
        await Assert.That(SnippetKey.ExtractStartCommentWebSnippet(new("<!-- web-snippet: https://example.com/file.cs#mysnippet -->", "path", 1), out var url, out var key)).IsTrue();
        await Assert.That(url).IsEqualTo("https://example.com/file.cs");
        await Assert.That(key).IsEqualTo("mysnippet");
    }

    [Test]
    public async Task MissingClosingComment_Throws()
    {
        var line = new Line("<!-- web-snippet: https://example.com/file.cs#mysnippet", "test.md", 10);
        var exception = Assert.Throws<SnippetException>(() => SnippetKey.ExtractStartCommentWebSnippet(line, out _, out _));
        await Assert.That(exception.Message).Contains("-->");
        await Assert.That(exception.Message).Contains("test.md");
    }
}