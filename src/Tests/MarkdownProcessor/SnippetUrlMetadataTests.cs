public class SnippetUrlMetadataTests
{
    const string url = "https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt";

    [Test]
    public Task FullFileUrl_LanguageOnlyOverride()
    {
        var output = SnippetVerifier.Render(
            DocumentConvention.SourceTransform,
            $"snippet: {url} (lang=cs)",
            null,
            null,
            null);
        return Verify(output);
    }

    [Test]
    public Task FullFileUrl_OverridesLanguageAndRendersExpressiveCode()
    {
        var output = SnippetVerifier.Render(
            DocumentConvention.SourceTransform,
            $"snippet: {url} (lang=cs title=code1.txt)",
            null,
            null,
            null);
        return Verify(output);
    }

    [Test]
    public Task FullFileUrl_MetadataWithoutLanguageUsesUrlLanguage()
    {
        var output = SnippetVerifier.Render(
            DocumentConvention.SourceTransform,
            $"snippet: {url} (title=code1.txt)",
            null,
            null,
            null);
        return Verify(output);
    }

    [Test]
    public async Task FullFileUrl_InvalidLanguageThrows()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            SnippetVerifier.Render(
                DocumentConvention.SourceTransform,
                $"snippet: {url} (lang=CS title=code1.txt)",
                null,
                null,
                null));
        await Assert.That(exception.Message).Contains("lang value must be lowercase alphanumeric.");
        await Assert.That(exception.Message).Contains(url);
    }

    [Test]
    public Task InPlaceFullFileUrl_PreservesMetadataInGeneratedMarker()
    {
        var output = SnippetVerifier.Render(
            DocumentConvention.InPlaceOverwrite,
            $"""
             <!-- snippet: {url} (lang=cs title=code1.txt) -->
             old content
             <!-- endSnippet -->
             """,
            null,
            null,
            null);
        return Verify(output);
    }
}
