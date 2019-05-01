using System;
using System.Collections.Generic;

namespace MarkdownSnippets
{
    public delegate void AppendSnippetGroupToMarkdown(string key, IEnumerable<Snippet> snippets, Action<string> appendLine);
}