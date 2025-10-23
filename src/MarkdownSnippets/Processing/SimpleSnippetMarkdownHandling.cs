namespace MarkdownSnippets;

/// <summary>
/// Simple markdown handling to be passed to <see cref="MarkdownProcessor"/>.
/// </summary>
public static class SimpleSnippetMarkdownHandling
{
    public static void Append(string key, IEnumerable<Snippet> snippets, Action<string> appendLine)
    {
        foreach (var snippet in snippets)
        {
            WriteSnippet(appendLine, snippet);
        }
    }

    static void WriteSnippet(Action<string> appendLine, Snippet snippet)
    {
        if (snippet.ExpressiveCode is null)
        {
            appendLine($"```{snippet.Language}");
        }
        else
        {
            appendLine($"```{snippet.Language} {snippet.ExpressiveCode}");
        }

        appendLine(snippet.Value);
        appendLine("```");
    }
}