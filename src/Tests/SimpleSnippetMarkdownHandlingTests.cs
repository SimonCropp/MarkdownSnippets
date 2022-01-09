using MarkdownSnippets;

[UsesVerify]
public class SimpleSnippetMarkdownHandlingTests
{
    [Fact]
    public Task Append()
    {
        var builder = new StringBuilder();
        var snippets = new List<Snippet> {Snippet.Build(1, 2, "theValue", "thekey", "thelanguage", "thePath")};
        using (var writer = new StringWriter(builder))
        {
            SimpleSnippetMarkdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verify(builder.ToString());
    }
}