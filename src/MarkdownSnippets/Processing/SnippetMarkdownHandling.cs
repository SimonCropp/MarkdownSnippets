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

        public void AppendGroup(string key, IEnumerable<Snippet> snippets, TextWriter writer)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(writer, nameof(writer));

            foreach (var snippet in snippets)
            {
                WriteSnippet(writer, snippet);
            }
        }

        void WriteSnippet(TextWriter writer, Snippet snippet)
        {
            writer.WriteLine($"```{snippet.Language}");
            writer.WriteLine(snippet.Value);
            writer.WriteLine("```");

            if (snippet.Path != null)
            {
                var path = snippet.Path.Replace(@"\", "/").ReplaceCaseless(rootDirectory, "");
                writer.WriteLine($"<sup>[snippet source]({path}#L{snippet.StartLine}-L{snippet.EndLine})</sup>");
            }
        }
    }
}