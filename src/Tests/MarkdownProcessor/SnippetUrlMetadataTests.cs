public class SnippetUrlMetadataTests
{
    const string url = "https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Directory.Packages.props";

    [Test]
    public async Task FullFileUrl_LanguageOnlyOverride()
    {
        var result = SnippetVerifier.Render(
            DocumentConvention.SourceTransform,
            $"snippet: {url} (lang=xml)",
            null,
            null,
            null);

        await Assert.That(result.result).Contains($"<!-- snippet: {url} (lang=xml) -->");
        await Assert.That(result.result).Contains("```xml\n");
    }

    [Test]
    public async Task FullFileUrl_OverridesLanguageAndRendersExpressiveCode()
    {
        var result = SnippetVerifier.Render(
            DocumentConvention.SourceTransform,
            $"snippet: {url} (lang=xml title=Directory.Packages.props)",
            null,
            null,
            null);

        await Assert.That(result.result).Contains($"<!-- snippet: {url} (lang=xml title=Directory.Packages.props) -->");
        await Assert.That(result.result).Contains("```xml title=Directory.Packages.props");
    }

    [Test]
    public async Task FullFileUrl_MetadataWithoutLanguageUsesUrlLanguage()
    {
        var result = SnippetVerifier.Render(
            DocumentConvention.SourceTransform,
            $"snippet: {url} (title=Directory.Packages.props)",
            null,
            null,
            null);

        await Assert.That(result.result).Contains("```props title=Directory.Packages.props");
    }

    [Test]
    public async Task FullFileUrl_InvalidLanguageThrows()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            SnippetVerifier.Render(
                DocumentConvention.SourceTransform,
                $"snippet: {url} (lang=XML title=Directory.Packages.props)",
                null,
                null,
                null));
        await Assert.That(exception.Message).Contains("lang value must be lowercase alphanumeric.");
        await Assert.That(exception.Message).Contains(url);
    }

    [Test]
    public async Task InPlaceFullFileUrl_PreservesMetadataInGeneratedMarker()
    {
        var result = SnippetVerifier.Render(
            DocumentConvention.InPlaceOverwrite,
            $"""
             <!-- snippet: {url} (lang=xml title=Directory.Packages.props) -->
             old content
             <!-- endSnippet -->
             """,
            null,
            null,
            null);

        await Assert.That(result.result).Contains($"<!-- snippet: {url} (lang=xml title=Directory.Packages.props) -->");
        await Assert.That(result.result).Contains("```xml title=Directory.Packages.props");
        await Assert.That(result.result).DoesNotContain("old content");
    }
}
