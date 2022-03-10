using MarkdownSnippets;

[UsesVerify]
public class SnippetExtensionsTests
{
    [Fact]
    public Task ToDictionary()
    {
        var snippets = new List<Snippet>
        {
            SnippetBuild("snippet1", "thePath"),
            SnippetBuild("snippet2", "thePath")
        };
        return Verify(snippets.ToDictionary());
    }

    [Fact]
    public Task ToDictionary_SameKey()
    {
        var snippets = new List<Snippet>
        {
            SnippetBuild("snippet1", null),
            SnippetBuild("snippet1", "thePath2"),
            SnippetBuild("snippet1", "thePath1")
        };
        return Verify(snippets.ToDictionary());
    }

    static Snippet SnippetBuild(string key, string? path) =>
        Snippet.Build(
            language: "language",
            startLine: 1,
            endLine: 2,
            value: "Snippet",
            key: key,
            path: path);
}