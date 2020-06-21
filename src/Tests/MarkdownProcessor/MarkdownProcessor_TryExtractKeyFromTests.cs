using MarkdownSnippets;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class MarkdownProcessor_TryExtractKeyFromTests
{
    [Fact]
    public void MissingSpaces()
    {
        var exception = Assert.Throws<MarkdownProcessingException>(() => SnippetKeyReader.TryExtractKeyFromLine(new Line("snippet:snippet","path",1), out _));
        Assert.True(exception.Message == "Invalid syntax for the snippet 'snippet': There must be a space before the start of the key.");
    }

    [Fact]
    public void WithDashes()
    {
        SnippetKeyReader.TryExtractKeyFromLine(new Line("snippet: my-code-snippet","path",1), out var key);
        Assert.Equal("my-code-snippet", key);
    }

    [Fact]
    public void Simple()
    {
        SnippetKeyReader.TryExtractKeyFromLine(new Line("snippet: snippet","path",1), out var key);
        Assert.Equal("snippet", key);
    }

    [Fact]
    public void ExtraSpace()
    {
        SnippetKeyReader.TryExtractKeyFromLine(new Line("snippet:  snippet   ","path",1), out var key);
        Assert.Equal("snippet", key);
    }
}