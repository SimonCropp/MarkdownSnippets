using System;
using System.Collections.Generic;
using System.IO;

namespace MarkdownSnippets
{
    /// <summary>
    /// Handling to be passed to <see cref="MarkdownProcessor"/>.
    /// </summary>
    public class SnippetMarkdownHandling
    {
        string rootDirectory;

        public SnippetMarkdownHandling(string rootDirectory)
        {
            Guard.AgainstNullAndEmpty(rootDirectory, nameof(rootDirectory));
            rootDirectory = Path.GetFullPath(rootDirectory);
            this.rootDirectory = rootDirectory.Replace(@"\", "/");
        }

        public void AppendGroup(string key, IEnumerable<Snippet> snippets, Action<string> appendLine)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(appendLine, nameof(appendLine));

            foreach (var snippet in snippets)
            {
                WriteSnippet(appendLine, snippet);
            }
        }

        void WriteSnippet(Action<string> appendLine, Snippet snippet)
        {
            appendLine($"```{snippet.Language}");
            appendLine(snippet.Value);
            appendLine("```");

            if (snippet.Path != null)
            {
                var path = snippet.Path.Replace(@"\", "/").ReplaceCaseless(rootDirectory, "");
                appendLine($"<sup>[snippet source]({path}#L{snippet.StartLine}-L{snippet.EndLine})</sup>");
            }
        }
    }
}