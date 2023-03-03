namespace MarkdownSnippets;

public delegate void AppendSnippetsToMarkdown(string key, IEnumerable<Snippet> snippets, AppendLine appendLine);