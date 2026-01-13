public class SnippetKey_ExtractStartCommentSnippet
{
    [Fact]
    public void WithDashes()
    {
        Assert.True(SnippetKey.ExtractStartCommentSnippet(new("<!-- snippet: my-code-snippet -->", "path", 1), out var key));
        Assert.Equal("my-code-snippet", key);
    }

    [Fact]
    public void Simple()
    {
        Assert.True(SnippetKey.ExtractStartCommentSnippet(new("<!-- snippet: snippet -->", "path", 1), out var key));
        Assert.Equal("snippet", key);
    }

    [Fact]
    public void MissingClosingComment_Throws()
    {
        var line = new Line("<!-- snippet: my-snippet", "test.md", 5);
        var exception = Assert.Throws<SnippetException>(() => SnippetKey.ExtractStartCommentSnippet(line, out _));
        Assert.Contains("-->", exception.Message);
        Assert.Contains("test.md", exception.Message);
    }
}