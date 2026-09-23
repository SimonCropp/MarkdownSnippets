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
}