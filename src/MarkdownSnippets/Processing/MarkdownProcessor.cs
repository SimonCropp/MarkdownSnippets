using System;
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
        List<string> snippetSourceFiles;

        public MarkdownProcessor(
            IReadOnlyDictionary<string, IReadOnlyList<Snippet>> snippets,
            AppendSnippetGroupToMarkdown appendSnippetGroup,
            IReadOnlyList<string> snippetSourceFiles)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(appendSnippetGroup, nameof(appendSnippetGroup));
            Guard.AgainstNull(snippetSourceFiles, nameof(snippetSourceFiles));
            this.snippets = snippets;
            this.appendSnippetGroup = appendSnippetGroup;
            InitSourceFiles(snippetSourceFiles);
        }

        public MarkdownProcessor(
            ReadSnippets snippets,
            AppendSnippetGroupToMarkdown appendSnippetGroup)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(appendSnippetGroup, nameof(appendSnippetGroup));
            this.snippets = snippets.ToDictionary();
            this.appendSnippetGroup = appendSnippetGroup;
            InitSourceFiles(snippets.Files);
        }

        public MarkdownProcessor(
            IEnumerable<Snippet> snippets,
            AppendSnippetGroupToMarkdown appendSnippetGroup,
            IReadOnlyList<string> snippetSourceFiles)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(appendSnippetGroup, nameof(appendSnippetGroup));
            Guard.AgainstNull(snippetSourceFiles, nameof(snippetSourceFiles));
            this.snippets = snippets.ToDictionary();
            this.appendSnippetGroup = appendSnippetGroup;
            InitSourceFiles(snippetSourceFiles);
        }

        void InitSourceFiles(IEnumerable<string> snippetSourceFiles)
        {
            this.snippetSourceFiles = snippetSourceFiles
                .Select(x => x.Replace('\\', '/'))
                .ToList();
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
                if (reader.Index == 1)
                {
                    writer.NewLine = reader.NewLine;
                } 
                if (TryProcessSnippetLine(writer, reader.Index, line, missing, usedSnippets))
                {
                    continue;
                }

                writer.WriteLine(line);
            }

            return new ProcessResult(
                missingSnippets: missing,
                usedSnippets: usedSnippets.Distinct().ToList());
        }

        bool TryProcessSnippetLine(TextWriter writer, int index, string line, List<MissingSnippet> missings, List<Snippet> used)
        {
            if (!SnippetKeyReader.TryExtractKeyFromLine(line, out var key))
            {
                return false;
            }

            writer.WriteLine($"<!-- snippet: {key} -->");

            if (TryGetFromFiles(key, out var snippetFromFiles))
            {
                appendSnippetGroup(key, snippetFromFiles, writer);
                writer.WriteLine($"<!-- endsnippet -->");
                used.AddRange(snippetFromFiles);
                return true;
            }

            var missing = new MissingSnippet(
                key: key,
                line: index);
            missings.Add(missing);
            var message = $"** Could not find snippet '{key}' **";
            writer.WriteLine(message);
            return true;
        }

        bool TryGetFromFiles(string key, out IReadOnlyList<Snippet> snippetFromFiles)
        {
            if (snippets.TryGetValue(key, out snippetFromFiles))
            {
                return true;
            }

            snippetFromFiles = FilesToSnippets(key);
            return snippetFromFiles.Any();
        }

        List<Snippet> FilesToSnippets(string key)
        {
            return snippetSourceFiles
                .Where(file => file.EndsWith(key, StringComparison.OrdinalIgnoreCase))
                .Select(file =>
                {
                    var allText = File.ReadAllText(file);
                    return Snippet.Build(
                        startLine: 1,
                        endLine: allText.LineCount(),
                        value: allText,
                        key: key,
                        language: Path.GetExtension(file).Substring(1),
                        path: file);
                })
                .ToList();
        }
    }
}