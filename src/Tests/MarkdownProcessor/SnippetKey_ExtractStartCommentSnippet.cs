public class SnippetKey_ExtractStartCommentSnippet
{
    [Test]
    public async Task WithDashes()
    {
        await Assert.That(SnippetKey.ExtractStartCommentSnippet(new("<!-- snippet: my-code-snippet -->", "path", 1), out var key)).IsTrue();
        await Assert.That(key).IsEqualTo("my-code-snippet");
    }

    [Test]
    public async Task Simple()
    {
        await Assert.That(SnippetKey.ExtractStartCommentSnippet(new("<!-- snippet: snippet -->", "path", 1), out var key)).IsTrue();
        await Assert.That(key).IsEqualTo("snippet");
    }

    [Test]
    public async Task MissingClosingComment_Throws()
    {
        var line = new Line("<!-- snippet: my-snippet", "test.md", 5);
        var exception = Assert.Throws<SnippetException>(() => SnippetKey.ExtractStartCommentSnippet(line, out _));
        await Assert.That(exception.Message).Contains("-->");
        await Assert.That(exception.Message).Contains("test.md");
    }

    [Test]
    public async Task UrlMetadata_IsExtractedFromGeneratedMarker()
    {
        const string url = "https://example.com/nuget.config?raw=1#config";
        await Assert.That(SnippetKey.ExtractStartCommentSnippet(
            new($"<!-- snippet: {url} (lang=xml title=nuget.config) -->", "path", 1),
            out var key,
            out var language,
            out var expressiveCode)).IsTrue();
        await Assert.That(key).IsEqualTo(url);
        await Assert.That(language).IsEqualTo("xml");
        await Assert.That(expressiveCode).IsEqualTo("title=nuget.config");
    }
}