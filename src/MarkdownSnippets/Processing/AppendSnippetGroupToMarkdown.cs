using System.Collections.Generic;
using System.IO;

namespace MarkdownSnippets
{
    public delegate void AppendSnippetGroupToMarkdown(string key, IReadOnlyList<Snippet> snippets, TextWriter writer);
}