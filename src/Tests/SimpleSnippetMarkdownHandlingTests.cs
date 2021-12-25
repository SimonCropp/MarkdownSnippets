using MarkdownSnippets;

[UsesVerify]
public class SimpleSnippetMarkdownHandlingTests
{
    [Fact]
    public Task Append()
    {
        StringBuilder builder = new();
        List<Snippet> snippets = new() {Snippet.Build(1, 2, "theValue", "thekey", "thelanguage", "thePath")};
        using (StringWriter writer = new(builder))
        {
            SimpleSnippetMarkdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verify(builder.ToString());
    }
}