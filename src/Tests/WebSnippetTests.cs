public class WebSnippetTests
{
    [Test]
    public async Task ExtractWebSnippet_ParsesCorrectly()
    {
        var line = new Line("web-snippet:https://example.com/file.cs#mysnippet", "", 1);
        await Assert.That(SnippetKey.ExtractWebSnippet(line, out var url, out var key)).IsTrue();
        await Assert.That(url).IsEqualTo("https://example.com/file.cs");
        await Assert.That(key).IsEqualTo("mysnippet");
    }

    [Test]
    public async Task ExtractWebSnippet_FailsWithoutHash()
    {
        var line = new Line("web-snippet:https://example.com/file.cs", "", 1);
        await Assert.That(SnippetKey.ExtractWebSnippet(line, out var _, out var _)).IsFalse();
    }
}
