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

        public string Apply(string input, string file = null)
        {
            Guard.AgainstNull(input, nameof(input));
            Guard.AgainstEmpty(file, nameof(file));
            var builder = new StringBuilder();
            using (var reader = new StringReader(input))
            using (var writer = new StringWriter(builder))
            {
                var processResult = Apply(reader, writer, file);
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
        public ProcessResult Apply(TextReader textReader, TextWriter writer, string file = null)
        {
            Guard.AgainstNull(textReader, nameof(textReader));
            Guard.AgainstNull(writer, nameof(writer));
            Guard.AgainstEmpty(file, nameof(file));
            var reader = new IndexReader(textReader);
            return Apply(writer, reader, file);
        }

        ProcessResult Apply(TextWriter writer, IndexReader reader, string file)
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

                if (TryProcessSnippetLine(writer.WriteLine, reader.Index, line, missing, usedSnippets, file))
                {
                    continue;
                }

                writer.WriteLine(line);
            }

            return new ProcessResult(
                missingSnippets: missing,
                usedSnippets: usedSnippets.Distinct().ToList());
        }

        bool TryProcessSnippetLine(Action<string> appendLine, int index, string line, List<MissingSnippet> missings, List<Snippet> used, string file)
        {
            if (!SnippetKeyReader.TryExtractKeyFromLine(line, file, index, out var key))
            {
                return false;
            }

            appendLine($"<!-- snippet: {key} -->");

            if (TryGetFromFiles(key, out var snippetFromFiles))
            {
                appendSnippetGroup(key, snippetFromFiles, appendLine);
                appendLine("<!-- endsnippet -->");
                used.AddRange(snippetFromFiles);
                return true;
            }

            var missing = new MissingSnippet(key, index, file);
            missings.Add(missing);
            appendLine($"** Could not find snippet '{key}' **");
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
            string keyWithDirChar;
            if (key.StartsWith("/"))
            {
                keyWithDirChar = key;
            }
            else
            {
                keyWithDirChar = "/" + key;
            }

            return snippetSourceFiles
                .Where(file => file.EndsWith(keyWithDirChar, StringComparison.OrdinalIgnoreCase))
                .Select(file =>
                {
                    var allText = ReadNonStartEndLines(file);
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

        static string ReadNonStartEndLines(string file)
        {
            var cleanedLines = File.ReadAllLines(file)
                .Where(x => !StartEndTester.IsStartOrEnd(x.TrimStart()));
            return string.Join(Environment.NewLine, cleanedLines);
        }
    }
}