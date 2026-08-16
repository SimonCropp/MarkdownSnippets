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

        return Verify(builder.ToString())
            .Snapshot(
                """
                <a id='snippet-thekey'></a>
                ```thelanguage
                theValue
                ```
                <sup><a href='#L1-L2' title='Snippet source file'>snippet source</a> | <a href='#snippet-thekey' title='Start of snippet'>anchor</a></sup>

                """);
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

        return Verify(builder.ToString())
            .Snapshot(
                """
                <a id='snippet-thekey'></a>
                ```thelanguage
                theValue
                ```
                <sup><a href='#snippet-thekey' title='Start of snippet'>anchor</a></sup>

                """);
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

        return Verify(builder.ToString())
            .Snapshot(
                """
                ```thelanguage
                theValue
                ```

                """);
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

        return Verify(builder.ToString())
            .Snapshot(
                """
                <a id='snippet-thekey'></a>
                ```thelanguage
                theValue
                ```
                <sup><a href='prefix-#L1-L2' title='Snippet source file'>snippet source</a> | <a href='#snippet-thekey' title='Start of snippet'>anchor</a></sup>

                """);
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

        return Verify(builder.ToString())
            .Snapshot(
                """
                <a id='snippet-thekey'></a>
                ```thelanguage
                theValue
                ```
                <sup><a href='#L1-L2' title='Snippet source file'>snippet source</a> | <a href='#snippet-thekey' title='Start of snippet'>anchor</a></sup>

                """);
    }

    [Fact]
    public Task AppendWebSnippet()
    {
        var builder = new StringBuilder();
        var webSnippet = Snippet.Build(
            startLine: 1,
            endLine: 2,
            value: "theValue",
            key: "mysnippet",
            language: "cs",
            path: "http://example.com/file.cs",
            expressiveCode: null);
        var markdownHandling = new SnippetMarkdownHandling(Environment.CurrentDirectory, LinkFormat.GitHub, false);
        using (var writer = new StringWriter(builder))
        {
            markdownHandling.Append("key1", new List<Snippet> { webSnippet }, writer.WriteLine);
        }

        return Verify(builder.ToString())
            .Snapshot(
                """
                <a id='snippet-http://example.com/file.cs%23mysnippet'></a>
                ```cs
                theValue
                ```
                <sup><a href='http://example.com/file.cs#mysnippet' title='Snippet source file'>anchor</a></sup>

                """);
    }

    [Fact]
    public Task AppendWebSnippetWithViewUrl()
    {
        var builder = new StringBuilder();
        var webSnippet = Snippet.Build(
            startLine: 5,
            endLine: 10,
            value: "theValue",
            key: "mysnippet",
            language: "cs",
            path: "http://example.com/raw/file.cs",
            expressiveCode: null,
            viewUrl: "https://github.com/user/repo/blob/main/file.cs");
        var markdownHandling = new SnippetMarkdownHandling(Environment.CurrentDirectory, LinkFormat.GitHub, false);
        using (var writer = new StringWriter(builder))
        {
            markdownHandling.Append("key1", new List<Snippet> { webSnippet }, writer.WriteLine);
        }

        return Verify(builder.ToString())
            .Snapshot(
                """
                <a id='snippet-http://example.com/raw/file.cs%23mysnippet'></a>
                ```cs
                theValue
                ```
                <sup><a href='https://github.com/user/repo/blob/main/file.cs#L5-L10' title='Snippet source file'>anchor</a></sup>

                """);
    }

    static List<Snippet> Snippets() =>
        [Snippet.Build(1, 2, "theValue", "thekey", "thelanguage", Environment.CurrentDirectory, expressiveCode: null)];
}