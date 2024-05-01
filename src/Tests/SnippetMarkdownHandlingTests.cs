using MarkdownSnippets;

public class SnippetMarkdownHandlingTests
{
    [Fact]
    public Task Append()
    {
        var builder = new StringBuilder();
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling(Environment.CurrentDirectory, LinkFormat.GitHub, false);
        using (var writer = new StringWriter(builder))
        {
            markdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verify(builder.ToString());
    }

    [Fact]
    public Task AppendOmitSourceLink()
    {
        var builder = new StringBuilder();
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling(Environment.CurrentDirectory, LinkFormat.None, false);
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
        var markdownHandling = new SnippetMarkdownHandling(Environment.CurrentDirectory, LinkFormat.GitHub, true);
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
        var markdownHandling = new SnippetMarkdownHandling(Environment.CurrentDirectory, LinkFormat.GitHub, false, "prefix-");
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
        var markdownHandling = new SnippetMarkdownHandling(Environment.CurrentDirectory, LinkFormat.GitHub, false);
        using (var writer = new StringWriter(builder))
        {
            markdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verify(builder.ToString());
    }

    static List<Snippet> Snippets() =>
        [Snippet.Build(1, 2, "theValue", "thekey", "thelanguage", Environment.CurrentDirectory)];
}