namespace MarkdownSnippets;

/// <summary>
/// Simple markdown handling to be passed to <see cref="MarkdownProcessor"/>.
/// </summary>
public static class SimpleSnippetMarkdownHandling
{
    public static string Append(string key, IEnumerable<Snippet> snippets)
    {
        var builder = new StringBuilder();
        foreach (var snippet in snippets)
        {
            builder.Append(WriteSnippet(snippet));
        }
        return builder.ToString().TrimEnd();
    }

    static string WriteSnippet(Snippet snippet) =>
        $"```{snippet.Language}\n{snippet.Value}\n```";
}