using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MarkdownSnippets
{
    /// <summary>
    /// Merges <see cref="Snippet"/>s with an input file/text.
    /// </summary>
    public class MarkdownProcessor
    {
        IReadOnlyDictionary<string, IReadOnlyList<Snippet>> snippets;
        AppendSnippetGroupToMarkdown appendSnippetGroup;

        public MarkdownProcessor(
            IReadOnlyDictionary<string, IReadOnlyList<Snippet>> snippets,
            AppendSnippetGroupToMarkdown appendSnippetGroup)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(appendSnippetGroup, nameof(appendSnippetGroup));
            this.snippets = snippets;
            this.appendSnippetGroup = appendSnippetGroup;
        }

        public MarkdownProcessor(
            IEnumerable<Snippet> snippets,
            AppendSnippetGroupToMarkdown appendSnippetGroup)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(appendSnippetGroup, nameof(appendSnippetGroup));
            this.snippets = snippets.ToDictionary();
            this.appendSnippetGroup = appendSnippetGroup;
        }

        public string Apply(string input)
        {
            Guard.AgainstNull(input, nameof(input));
            var builder = new StringBuilder();
            using (var reader = new StringReader(input))
            using (var writer = new StringWriter(builder))
            {
                var processResult = Apply(reader, writer);
                var missing = processResult.MissingSnippets;
                if (missing.Any())
                {
                    throw new MissingSnippetsException(missing);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Apply to <paramref name="writer"/>.
        /// </summary>
        public ProcessResult Apply(TextReader textReader, TextWriter writer)
        {
            Guard.AgainstNull(textReader, nameof(textReader));
            Guard.AgainstNull(writer, nameof(writer));
            var reader = new IndexReader(textReader);
            return Apply(writer, reader);
        }

        ProcessResult Apply(TextWriter writer, IndexReader reader)
        {
            var missing = new List<MissingSnippet>();
            var usedSnippets = new List<Snippet>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (TryProcessSnippetLine(writer, reader, line, missing, usedSnippets))
                {
                    continue;
                }
                writer.WriteLine(line);
            }
            return new ProcessResult(
                missingSnippets: missing,
                usedSnippets: usedSnippets.Distinct().ToList());
        }

        bool TryProcessSnippetLine(TextWriter writer, IndexReader reader, string line, List<MissingSnippet> missings, List<Snippet> used)
        {
            if (!SnippetKeyReader.TryExtractKeyFromLine(line, out var key))
            {
                return false;
            }
            writer.WriteLine($"<!-- snippet: {key} -->");

            if (!snippets.TryGetValue(key, out var group))
            {
                var missing = new MissingSnippet(
                    key: key,
                    line: reader.Index);
                missings.Add(missing);
                var message = $"** Could not find snippet '{key}' **";
                writer.WriteLine(message);
                return true;
            }

            appendSnippetGroup(key, group, writer);
            writer.WriteLine($"<!-- endsnippet -->");
            used.AddRange(group);
            return true;
        }
    }
}