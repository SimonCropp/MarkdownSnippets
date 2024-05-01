using MarkdownSnippets;

public class SnippetMarkdownHandlingTests
{
    [Fact]
    public Task Append()
    {
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling(Environment.CurrentDirectory, LinkFormat.GitHub, false);
        return Verify(markdownHandling.Append("key1", snippets));
    }

    [Fact]
    public Task AppendOmitSourceLink()
    {
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling(Environment.CurrentDirectory, LinkFormat.None, false);
        return Verify(markdownHandling.Append("key1", snippets));
    }

    [Fact]
    public Task AppendOmitSnippetLinks()
    {
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling(Environment.CurrentDirectory, LinkFormat.GitHub, true);
        return Verify(markdownHandling.Append("key1", snippets));
    }

    [Fact]
    public Task AppendPrefixed()
    {
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling(Environment.CurrentDirectory, LinkFormat.GitHub, false, "prefix-");
        return Verify(markdownHandling.Append("key1", snippets));
    }

    [Fact]
    public Task AppendHashed()
    {
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling(Environment.CurrentDirectory, LinkFormat.GitHub, false);
        return Verify(markdownHandling.Append("key1", snippets));
    }

    static List<Snippet> Snippets() =>
        [Snippet.Build(1, 2, "theValue", "thekey", "thelanguage", Environment.CurrentDirectory)];
}