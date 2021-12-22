using VerifyXunit;
using MarkdownSnippets;
using Xunit;

[UsesVerify]
public class SnippetMarkdownHandlingTests
{
    [Fact]
    public Task Append()
    {
        var builder = new StringBuilder();
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling("c:/dir/", LinkFormat.GitHub, false, false);
        using (var writer = new StringWriter(builder))
        {
            markdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verify(builder.ToString());
    }

    [Fact]
    public Task AppendOmitSnippetLinks()
    {
        var builder = new StringBuilder();
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling("c:/dir/", LinkFormat.GitHub, false, true);
        using (var writer = new StringWriter(builder))
        {
            markdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verify(builder.ToString());
    }

    [Fact]
    public Task AppendPrefixed()
    {
        var builder = new StringBuilder();
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling("c:/dir/", LinkFormat.GitHub, false, false, "prefix-");
        using (var writer = new StringWriter(builder))
        {
            markdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verify(builder.ToString());
    }

    [Fact]
    public Task AppendHashed()
    {
        var builder = new StringBuilder();
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling("c:/dir/", LinkFormat.GitHub, true, false);
        using (var writer = new StringWriter(builder))
        {
            markdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verify(builder.ToString());
    }

    static List<Snippet> Snippets()
    {
        return new()
        {
            Snippet.Build(1, 2, "theValue", "thekey", "thelanguage", "c:/dir/thePath")
        };
    }
}