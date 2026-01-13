public class SnippetKey_ExtractStartCommentWebSnippet
{
    [Fact]
    public void Simple()
    {
        Assert.True(SnippetKey.ExtractStartCommentWebSnippet(new("<!-- web-snippet: https://example.com/file.cs#mysnippet -->", "path", 1), out var url, out var key));
        Assert.Equal("https://example.com/file.cs", url);
        Assert.Equal("mysnippet", key);
    }

    [Fact]
    public void MissingClosingComment_Throws()
    {
        var line = new Line("<!-- web-snippet: https://example.com/file.cs#mysnippet", "test.md", 10);
        var exception = Assert.Throws<SnippetException>(() => SnippetKey.ExtractStartCommentWebSnippet(line, out _, out _));
        Assert.Contains("-->", exception.Message);
        Assert.Contains("test.md", exception.Message);
    }
}