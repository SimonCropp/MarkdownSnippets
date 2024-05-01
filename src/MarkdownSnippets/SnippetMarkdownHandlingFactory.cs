
static class SnippetMarkdownHandlingFactory
{
    public static void Set(AppendSnippetsToMarkdown append) =>
        Append = append;

    public static AppendSnippetsToMarkdown? Append { get; set; }
}