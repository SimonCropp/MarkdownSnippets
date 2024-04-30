using MarkdownSnippets;

public class SimpleSnippetMarkdownHandlingTests
{
    [Fact]
    public Task Append()
    {
        var snippets = new List<Snippet> {Snippet.Build(1, 2, "theValue", "thekey", "thelanguage", "thePath")};
        return Verify(SimpleSnippetMarkdownHandling.Append("key1", snippets));
    }
}