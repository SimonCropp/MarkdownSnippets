using System.Collections.Generic;
using System.IO;

namespace MarkdownSnippets
{
    public delegate void AppendSnippetGroupToMarkdown(string key, IEnumerable<Snippet> snippets, TextWriter writer);
}