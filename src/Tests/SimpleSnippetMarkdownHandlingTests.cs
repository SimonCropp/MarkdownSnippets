public class SimpleSnippetMarkdownHandlingTests
{
    [Fact]
    public Task Append()
    {
        var builder = new StringBuilder();
        var snippets = new List<Snippet> {Snippet.Build(1, 2, "theValue", "thekey", "thelanguage", "thePath", null)};
        using (var writer = new StringWriter(builder))
        {
            SimpleSnippetMarkdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verify(builder.ToString());
    }

    [Fact]
    public Task ExpressiveCode()
    {
        var builder = new StringBuilder();
        var snippets = new List<Snippet> {Snippet.Build(1, 2, """Console.WriteLine("Hello World");""", "thekey", "cs", "thePath", """title="HelloWorld.cs" {1}""")};
        using (var writer = new StringWriter(builder))
        {
            SimpleSnippetMarkdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verify(builder.ToString());
    }
}