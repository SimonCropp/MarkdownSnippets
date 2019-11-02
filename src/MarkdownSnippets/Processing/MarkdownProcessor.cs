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
        bool writeHeader;
        string? header;
        int tocLevel;
        List<string> tocExcludes;
        List<string> snippetSourceFiles;
        GetIncludeLines getIncludeLines;

        public MarkdownProcessor(
            IReadOnlyDictionary<string, IReadOnlyList<Snippet>> snippets,
            AppendSnippetGroupToMarkdown appendSnippetGroup,
            IReadOnlyList<string> snippetSourceFiles,
            int tocLevel,
            bool writeHeader,
            string? header = null,
            IEnumerable<string>? tocExcludes = null,
            GetIncludeLines? getIncludeLines = null)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(appendSnippetGroup, nameof(appendSnippetGroup));
            Guard.AgainstNull(snippetSourceFiles, nameof(snippetSourceFiles));
            Guard.AgainstEmpty(header, nameof(header));
            Guard.AgainstNegativeAndZero(tocLevel, nameof(tocLevel));
            this.snippets = snippets;
            this.appendSnippetGroup = appendSnippetGroup;
            this.writeHeader = writeHeader;
            this.header = header;
            this.tocLevel = tocLevel;
            if (getIncludeLines == null)
            {
                this.getIncludeLines = key => throw new Exception($"No GetIncludeLines defined. Key: {key}");
            }
            else
            {
                this.getIncludeLines = getIncludeLines;
            }
            if (tocExcludes == null)
            {
                this.tocExcludes = new List<string>();
            }
            else
            {
                this.tocExcludes = tocExcludes.ToList();
            }

            this.snippetSourceFiles = snippetSourceFiles
                .Select(x => x.Replace('\\', '/'))
                .ToList();
        }

        public string Apply(string input, string? file = null)
        {
            Guard.AgainstNull(input, nameof(input));
            Guard.AgainstEmpty(file, nameof(file));
            var builder = StringBuilderCache.Acquire();
            try
            {
                using var reader = new StringReader(input);
                using var writer = new StringWriter(builder);
                var processResult = Apply(reader, writer, file);
                var missing = processResult.MissingSnippets;
                if (missing.Any())
                {
                    throw new MissingSnippetsException(missing);
                }
                return builder.ToString();
            }
            finally
            {
                StringBuilderCache.Release(builder);
            }
        }

        /// <summary>
        /// Apply to <paramref name="writer"/>.
        /// </summary>
        public ProcessResult Apply(TextReader textReader, TextWriter writer, string? file = null)
        {
            Guard.AgainstNull(textReader, nameof(textReader));
            Guard.AgainstNull(writer, nameof(writer));
            Guard.AgainstEmpty(file, nameof(file));
            var (lines, newLine) = LineReader.ReadAllLines(textReader, null);
            writer.NewLine = newLine;
            var result = Apply(lines, newLine, file);
            foreach (var line in lines)
            {
                writer.WriteLine(line.Current);
            }

            return result;
        }

        internal ProcessResult Apply(List<Line> lines, string newLine, string? relativePath)
        {
            var missing = new List<MissingSnippet>();
            var usedSnippets = new List<Snippet>();
            var builder = new StringBuilder();
            Line? tocLine = null;
            var headerLines = new List<Line>();
            for (var index = 0; index < lines.Count; index++)
            {
                var line = lines[index];

                var current = line.Current;
                if (current.StartsWith("include: "))
                {
                    var includeLines = getIncludeLines(current.Substring(9));
                    line.Current = $"<!-- {current} -->";
                    for (var includeIndex = 0; includeIndex < includeLines.Count; includeIndex++)
                    {
                        var includeLine = includeLines[includeIndex];
                        //todo: path of include
                        lines.Insert(index + includeIndex + 1, new Line(includeLine, null, includeIndex));
                    }
                }

                if (current.StartsWith("#"))
                {
                    if (tocLine != null)
                    {
                        headerLines.Add(line);
                    }

                    continue;
                }

                if (current == "toc")
                {
                    tocLine = line;
                    continue;
                }

                if (SnippetKeyReader.TryExtractKeyFromLine(line, out var key))
                {
                    builder.Clear();

                    void AppendLine(string s)
                    {
                        builder.Append(s);
                        builder.Append(newLine);
                    }

                    ProcessSnippetLine(AppendLine, missing, usedSnippets, key, line);
                    builder.TrimEnd();
                    line.Current = builder.ToString();
                }
            }

            if (writeHeader)
            {
                lines.Insert(0, new Line(HeaderWriter.WriteHeader(relativePath!, header, newLine), "", 0));
            }

            if (tocLine != null)
            {
                tocLine.Current = TocBuilder.BuildToc(headerLines, tocLevel, tocExcludes, newLine);
            }

            return new ProcessResult(
                missingSnippets: missing,
                usedSnippets: usedSnippets.Distinct().ToList());
        }


        void ProcessSnippetLine(Action<string> appendLine, List<MissingSnippet> missings, List<Snippet> used, string key, Line line)
        {
            appendLine($"<!-- snippet: {key} -->");

            if (TryGetSnippets(key, out var snippetsForKey))
            {
                appendSnippetGroup(key, snippetsForKey, appendLine);
                appendLine("<!-- endsnippet -->");
                used.AddRange(snippetsForKey);
                return;
            }

            var missing = new MissingSnippet(key, line.LineNumber, line.Path);
            missings.Add(missing);
            appendLine($"** Could not find snippet '{key}' **");
        }

        bool TryGetSnippets(string key, out IReadOnlyList<Snippet> snippetsForKey)
        {
            if (snippets.TryGetValue(key, out snippetsForKey))
            {
                return true;
            }

            snippetsForKey = FilesToSnippets(key);
            return snippetsForKey.Any();
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
                    var (text, lineCount) = ReadNonStartEndLines(file);
                    return Snippet.Build(
                        startLine: 1,
                        endLine: lineCount,
                        value: text,
                        key: key,
                        language: Path.GetExtension(file).Substring(1),
                        path: file);
                })
                .ToList();
        }

        static (string text, int lineCount) ReadNonStartEndLines(string file)
        {
            var cleanedLines = File.ReadAllLines(file)
                .Where(x => !StartEndTester.IsStartOrEnd(x.TrimStart())).ToList();
            return (string.Join(Environment.NewLine, cleanedLines), cleanedLines.Count);
        }
    }
}