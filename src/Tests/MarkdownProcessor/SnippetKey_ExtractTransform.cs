public class SnippetKey_ExtractTransform
{
    [Fact]
    public void MissingSpaces()
    {
        Assert.True( new NewSnippet().ExtractSnippet(new("snippet:snippet", "path", 1), out var key));
        Assert.Equal("snippet", key);
    }

    [Fact]
    public void WithDashes()
    {
        Assert.True(new NewSnippet().ExtractSnippet(new("snippet: my-code-snippet", "path", 1), out var key));
        Assert.Equal("my-code-snippet", key);
    }

    [Fact]
    public void Simple()
    {
        Assert.True(new NewSnippet().ExtractSnippet(new("snippet: snippet", "path", 1), out var key));
        Assert.Equal("snippet", key);
    }

    [Fact]
    public void ExtraSpace()
    {
        Assert.True(new NewSnippet().ExtractSnippet(new("snippet:  snippet   ", "path", 1), out var key));
        Assert.Equal("snippet", key);
    }
}