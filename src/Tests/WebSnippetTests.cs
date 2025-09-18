public class WebSnippetTests
{
    [Fact]
    public void ExtractWebSnippet_ParsesCorrectly()
    {
        var line = new Line("web-snippet:https://example.com/file.cs#mysnippet", "", 1);
        Assert.True(SnippetKey.ExtractWebSnippet(line, out var url, out var key));
        Assert.Equal("https://example.com/file.cs", url);
        Assert.Equal("mysnippet", key);
    }

    [Fact]
    public void ExtractWebSnippet_FailsWithoutHash()
    {
        var line = new Line("web-snippet:https://example.com/file.cs", "", 1);
        Assert.False(SnippetKey.ExtractWebSnippet(line, out var _, out var _));
    }
}
