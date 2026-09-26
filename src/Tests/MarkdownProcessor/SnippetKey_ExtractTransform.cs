public class SnippetKey_ExtractTransform
{
    [Test]
    public async Task MissingSpaces()
    {
        await Assert.That(SnippetKey.ExtractSnippet(new("snippet:snippet", "path", 1), out var key)).IsTrue();
        await Assert.That(key).IsEqualTo("snippet");
    }

    [Test]
    public async Task WithDashes()
    {
        await Assert.That(SnippetKey.ExtractSnippet(new("snippet: my-code-snippet", "path", 1), out var key)).IsTrue();
        await Assert.That(key).IsEqualTo("my-code-snippet");
    }

    [Test]
    public async Task Simple()
    {
        await Assert.That(SnippetKey.ExtractSnippet(new("snippet: snippet", "path", 1), out var key)).IsTrue();
        await Assert.That(key).IsEqualTo("snippet");
    }

    [Test]
    public async Task ExtraSpace()
    {
        await Assert.That(SnippetKey.ExtractSnippet(new("snippet:  snippet   ", "path", 1), out var key)).IsTrue();
        await Assert.That(key).IsEqualTo("snippet");
    }

    [Test]
    public async Task UrlMetadata_PreservesQueryAndFragment()
    {
        const string url = "https://example.com/nuget.config?raw=1#config";
        await Assert.That(SnippetKey.ExtractSnippet(
            new($"snippet: {url} (lang=xml title=nuget.config {{1}})", "path", 1),
            out var key,
            out var language,
            out var expressiveCode)).IsTrue();
        await Assert.That(key).IsEqualTo(url);
        await Assert.That(language).IsEqualTo("xml");
        await Assert.That(expressiveCode).IsEqualTo("title=nuget.config {1}");
    }

    [Test]
    public async Task UrlMetadata_WithoutLanguageIsExpressiveCode()
    {
        const string url = "https://example.com/nuget.config";
        await Assert.That(SnippetKey.ExtractSnippet(
            new($"snippet: {url} (title=nuget.config)", "path", 1),
            out var key,
            out var language,
            out var expressiveCode)).IsTrue();
        await Assert.That(key).IsEqualTo(url);
        await Assert.That(language).IsNull();
        await Assert.That(expressiveCode).IsEqualTo("title=nuget.config");
    }

    [Test]
    public async Task UrlMetadata_PreservesParenthesesInsideExpressiveCode()
    {
        const string url = "https://example.com/nuget.config";
        await Assert.That(SnippetKey.ExtractSnippet(
            new($"snippet: {url} (title=\"NuGet (packages)\")", "path", 1),
            out var key,
            out var language,
            out var expressiveCode)).IsTrue();
        await Assert.That(key).IsEqualTo(url);
        await Assert.That(language).IsNull();
        await Assert.That(expressiveCode).IsEqualTo("title=\"NuGet (packages)\"");
    }

    [Test]
    public async Task UrlMetadata_InvalidLanguageThrowsLikeSourceSnippetMetadata()
    {
        const string url = "https://example.com/nuget.config";
        var exception = Assert.Throws<SnippetReadingException>(() =>
            SnippetKey.ExtractSnippet(
                new($"snippet: {url} (lang=XML title=nuget.config)", "path", 1),
                out _,
                out _,
                out _));
        await Assert.That(exception.Message).Contains("lang value must be lowercase alphanumeric.");
        await Assert.That(exception.Message).Contains(url);
        await Assert.That(exception.Message).Contains("Line: snippet:");
    }

    [Test]
    public async Task UrlMetadata_EmptyLanguageThrowsLikeSourceSnippetMetadata()
    {
        const string url = "https://example.com/nuget.config";
        var exception = Assert.Throws<SnippetReadingException>(() =>
            SnippetKey.ExtractSnippet(
                new($"snippet: {url} (lang= title=nuget.config)", "path", 1),
                out _,
                out _,
                out _));
        await Assert.That(exception.Message).Contains("lang= must have a value.");
        await Assert.That(exception.Message).Contains(url);
    }

    [Test]
    public async Task MetadataOnNonUrlKeyRemainsPartOfTheKey()
    {
        const string keyText = "snippet (lang=xml title=snippet)";
        await Assert.That(SnippetKey.ExtractSnippet(
            new($"snippet: {keyText}", "path", 1),
            out var key,
            out var language,
            out var expressiveCode)).IsTrue();
        await Assert.That(key).IsEqualTo(keyText);
        await Assert.That(language).IsNull();
        await Assert.That(expressiveCode).IsNull();
    }
}