using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class MarkdownProcessor_TryExtractKeyFromTests :
    TestBase
{
    [Fact]
    public void MissingSpaces()
    {
        var exception = Assert.Throws<MarkdownProcessingException>(() => SnippetKeyReader.TryExtractKeyFromLine("snippet:snippet", null, 1, out _));
        Assert.True(exception.Message == "Invalid syntax for the snippet 'snippet': There must be a space before the start of the key.");
    }

    [Fact]
    public void WithDashes()
    {
        SnippetKeyReader.TryExtractKeyFromLine("snippet: my-code-snippet", null, 1, out var key);
        Assert.Equal("my-code-snippet", key);
    }

    [Fact]
    public void Simple()
    {
        SnippetKeyReader.TryExtractKeyFromLine("snippet: snippet", null, 1, out var key);
        Assert.Equal("snippet", key);
    }

    [Fact]
    public void ExtraSpace()
    {
        SnippetKeyReader.TryExtractKeyFromLine("snippet:  snippet   ", null, 1, out var key);
        Assert.Equal("snippet", key);
    }

    public MarkdownProcessor_TryExtractKeyFromTests(ITestOutputHelper output) :
        base(output)
    {
    }
}