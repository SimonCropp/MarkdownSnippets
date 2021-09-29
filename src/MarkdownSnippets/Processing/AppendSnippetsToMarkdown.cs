namespace MarkdownSnippets;

public delegate void AppendSnippetsToMarkdown(string key, IEnumerable<Snippet> snippets, Action<string> appendLine);