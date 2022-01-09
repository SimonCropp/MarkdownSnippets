public class SnippetKey_ExtractTransform
{
    [Fact]
    public void MissingSpaces()
    {
        Assert.True( SnippetKey.ExtractSnippet(new Line("snippet:snippet", "path", 1), out var key));
        Assert.Equal("snippet", key);
    }

    [Fact]
    public void WithDashes()
    {
        Assert.True(SnippetKey.ExtractSnippet(new Line("snippet: my-code-snippet", "path", 1), out var key));
        Assert.Equal("my-code-snippet", key);
    }

    [Fact]
    public void Simple()
    {
        Assert.True(SnippetKey.ExtractSnippet(new Line("snippet: snippet", "path", 1), out var key));
        Assert.Equal("snippet", key);
    }

    [Fact]
    public void ExtraSpace()
    {
        Assert.True(SnippetKey.ExtractSnippet(new Line("snippet:  snippet   ", "path", 1), out var key));
        Assert.Equal("snippet", key);
    }
}