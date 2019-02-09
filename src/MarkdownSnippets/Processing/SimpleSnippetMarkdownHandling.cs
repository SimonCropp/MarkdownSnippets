using System.Collections.Generic;
using System.IO;

namespace MarkdownSnippets
{
    /// <summary>
    /// Simple markdown handling to be passed to <see cref="MarkdownProcessor"/>.
    /// </summary>
    public static class SimpleSnippetMarkdownHandling
    {
        public static void AppendGroup(string key, IEnumerable<Snippet> snippets, TextWriter writer)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(writer, nameof(writer));

            foreach (var snippet in snippets)
            {
                WriteSnippet(writer, snippet);
            }
        }

        static void WriteSnippet(TextWriter writer, Snippet snippet)
        {
            var format = $@"```{snippet.Language}
{snippet.Value}
```";
            writer.WriteLine(format);
        }
    }
}