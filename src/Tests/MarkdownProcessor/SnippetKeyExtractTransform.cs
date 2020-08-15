using MarkdownSnippets;
using Xunit;

public class SnippetKeyExtractTransform
{
    [Fact]
    public void MissingSpaces()
    {
        var exception = Assert.Throws<MarkdownProcessingException>(() => SnippetKey.ExtractTransform(new Line("snippet:snippet", "path", 1), out _));
        Assert.True(exception.Message == "Invalid syntax for the snippet 'snippet': There must be a space before the start of the key.");
    }

    [Fact]
    public void WithDashes()
    {
        SnippetKey.ExtractTransform(new Line("snippet: my-code-snippet", "path", 1), out var key);
        Assert.Equal("my-code-snippet", key);
    }

    [Fact]
    public void Simple()
    {
        SnippetKey.ExtractTransform(new Line("snippet: snippet", "path", 1), out var key);
        Assert.Equal("snippet", key);
    }

    [Fact]
    public void ExtraSpace()
    {
        SnippetKey.ExtractTransform(new Line("snippet:  snippet   ", "path", 1), out var key);
        Assert.Equal("snippet", key);
    }
}