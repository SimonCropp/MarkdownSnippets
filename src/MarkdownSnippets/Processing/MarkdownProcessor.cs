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
        DocumentConvention convention;
        IReadOnlyDictionary<string, IReadOnlyList<Snippet>> snippets;
        AppendSnippetsToMarkdown appendSnippets;
        bool writeHeader;
        bool validateContent;
        string? header;
        int tocLevel;
        List<string> tocExcludes;
        List<string> snippetSourceFiles;
        IncludeProcessor includeProcessor;

        static List<string> validationExcludes = new List<string>
        {
            "code_of_conduct",
            ".github",
            "license"
        };

        string rootDirectory;

        public MarkdownProcessor(
            DocumentConvention convention,
            IReadOnlyDictionary<string, IReadOnlyList<Snippet>> snippets,
            IReadOnlyList<Include> includes,
            AppendSnippetsToMarkdown appendSnippets,
            IReadOnlyList<string> snippetSourceFiles,
            int tocLevel,
            bool writeHeader,
            string rootDirectory,
            bool validateContent,
            string? header = null,
            IEnumerable<string>? tocExcludes = null)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            Guard.AgainstNull(appendSnippets, nameof(appendSnippets));
            Guard.AgainstNull(snippetSourceFiles, nameof(snippetSourceFiles));
            Guard.AgainstNull(includes, nameof(includes));
            Guard.AgainstEmpty(header, nameof(header));
            Guard.AgainstNegativeAndZero(tocLevel, nameof(tocLevel));
            Guard.AgainstNullAndEmpty(rootDirectory, nameof(rootDirectory));

            if (convention == DocumentConvention.InPlaceOverwrite && writeHeader)
            {
                throw new SnippetException("WriteHeader is not allowed with InPlaceOverwrite convention.");
            }

            this.rootDirectory = Path.GetFullPath(rootDirectory);
            this.convention = convention;
            this.snippets = snippets;
            this.appendSnippets = appendSnippets;
            this.writeHeader = writeHeader;
            this.validateContent = validateContent;
            this.header = header;
            this.tocLevel = tocLevel;
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
            includeProcessor = new IncludeProcessor(convention, includes, rootDirectory);
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
            var (lines, newLine) = Lines.ReadAllLines(textReader, null);
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
            var missingSnippets = new List<MissingSnippet>();
            var validationErrors = new List<ValidationError>();
            var missingIncludes = new List<MissingInclude>();
            var usedSnippets = new List<Snippet>();
            var usedIncludes = new List<Include>();
            var builder = new StringBuilder();
            Line? tocLine = null;

            void AppendLine(string s)
            {
                builder.Append(s);
                builder.Append(newLine);
            }

            var headerLines = new List<Line>();
            for (var index = 0; index < lines.Count; index++)
            {
                var line = lines[index];

                if (ValidateContent(relativePath, line, validationErrors))
                {
                    continue;
                }

                if (includeProcessor.TryProcessInclude(lines, line, usedIncludes, index, missingIncludes))
                {
                    continue;
                }

                if (line.Current.StartsWith("#"))
                {
                    if (tocLine != null)
                    {
                        headerLines.Add(line);
                    }

                    continue;
                }

                if (line.Current == "toc")
                {
                    tocLine = line;
                    continue;
                }


                void AppendSnippet(string key1)
                {
                    builder.Clear();
                    ProcessSnippetLine(AppendLine, missingSnippets, usedSnippets, key1, relativePath, line);
                    builder.TrimEnd();
                    line.Current = builder.ToString();
                }

                if (SnippetKey.ExtractSnippet(line, out var key))
                {
                    AppendSnippet(key);
                    continue;
                }

                if (convention == DocumentConvention.SourceTransform)
                {
                    continue;
                }

                if (SnippetKey.ExtractStartCommentSnippet(line, out key))
                {
                    AppendSnippet(key);

                    index++;

                    lines.RemoveUntil(
                        index,
                        "<!-- endSnippet -->",
                        relativePath,
                        line);
                    continue;
                }

                if (line.Current == "<!-- toc -->")
                {
                    tocLine = line;

                    index++;

                    lines.RemoveUntil(index, "<!-- endToc -->", relativePath, line);

                    continue;
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
                missingSnippets: missingSnippets,
                usedSnippets: usedSnippets.Distinct().ToList(),
                usedIncludes: usedIncludes.Distinct().ToList(),
                missingIncludes: missingIncludes,
                validationErrors: validationErrors);
        }

        bool ValidateContent(string? relativePath, Line line, List<ValidationError> validationErrors)
        {
            if (!validateContent)
            {
                return false;
            }

            if (relativePath != null &&
                validationExcludes.Any(relativePath.Contains))
            {
                return false;
            }

            var errors = ContentValidation.Verify(line.Original).ToList();
            if (!errors.Any())
            {
                return false;
            }

            validationErrors.AddRange(errors.Select(error => new ValidationError(error.error, line.LineNumber, error.column, line.Path)));
            return true;
        }

        void ProcessSnippetLine(Action<string> appendLine, List<MissingSnippet> missings, List<Snippet> used, string key, string? relativePath, Line line)
        {
            appendLine($"<!-- snippet: {key} -->");

            if (TryGetSnippets(key, relativePath, line.Path, out var snippetsForKey))
            {
                appendSnippets(key, snippetsForKey, appendLine);
                appendLine("<!-- endSnippet -->");
                used.AddRange(snippetsForKey);
                return;
            }

            var missing = new MissingSnippet(key, line.LineNumber, line.Path);
            missings.Add(missing);
            appendLine($"** Could not find snippet '{key}' **");
        }

        bool TryGetSnippets(string key, string? relativePath, string? linePath, out IReadOnlyList<Snippet> snippetsForKey)
        {
            if (snippets.TryGetValue(key, out snippetsForKey))
            {
                return true;
            }

            if (key.StartsWith("http"))
            {
                return GetForHttp(key, out snippetsForKey);
            }

            return FilesToSnippets(key, relativePath, linePath, out snippetsForKey);
        }

        bool FilesToSnippets(string key, string? relativePath, string? linePath, out IReadOnlyList<Snippet> snippetsForKey)
        {
            var keyWithDirChar = PrependSlash(key);

            snippetsForKey = snippetSourceFiles
                .Where(file => file.EndsWith(keyWithDirChar, StringComparison.OrdinalIgnoreCase))
                .Select(file => FileToSnippet(key, file, file))
                .ToList();
            if (snippetsForKey.Any())
            {
                return true;
            }

            if (FindPath(rootDirectory, key, relativePath, linePath, out var path))
            {
                snippetsForKey = SnippetsForFile(key, path);
                return true;
            }

            snippetsForKey = null!;
            return false;
        }

        static bool FindPath(string rootDirectory, string key, string? relativePath, string? linePath, out string path)
        {
            if (!key.Contains("."))
            {
                path = null!;
                return false;
            }

            var relativeToRoot = Path.Combine(rootDirectory, key);
            if (File.Exists(relativeToRoot))
            {
                path= relativeToRoot;
                return true;
            }

            var documentDirectory = Path.GetDirectoryName(relativePath);
            if (documentDirectory != null)
            {
                var relativeToDocument = Path.Combine(rootDirectory, documentDirectory.Trim('/', '\\'), key);
                if (File.Exists(relativeToDocument))
                {
                    path = relativeToDocument;
                    return true;
                }
            }

            var lineDirectory = Path.GetDirectoryName(linePath);
            if (lineDirectory != null)
            {
                var relativeToLine = Path.Combine(lineDirectory, key);
                if (File.Exists(relativeToLine))
                {
                    path =relativeToLine;
                    return true;
                }
            }

            if (File.Exists(key))
            {
                path = key;
                return true;
            }

            path = null!;
            return false;
        }

        static string PrependSlash(string key)
        {
            if (key.StartsWith("/"))
            {
                return key;
            }

            return "/" + key;
        }

        static List<Snippet> SnippetsForFile(string key, string relativeToRoot)
        {
            return new List<Snippet>
            {
                FileToSnippet(key, relativeToRoot, null)
            };
        }

        static bool GetForHttp(string key, out IReadOnlyList<Snippet> snippetsForKey)
        {
            var (success, path) = Downloader.DownloadFile(key).GetAwaiter().GetResult();
            if (!success)
            {
                snippetsForKey = null!;
                return false;
            }

            snippetsForKey = SnippetsForFile(key, path!);
            return true;
        }

        static Snippet FileToSnippet(string key, string file, string? path)
        {
            var (text, lineCount) = ReadNonStartEndLines(file);

            if (lineCount == 0)
            {
                lineCount++;
            }

            return Snippet.Build(
                startLine: 1,
                endLine: lineCount,
                value: text,
                key: key,
                language: Path.GetExtension(file).Substring(1),
                path: path);
        }

        static (string text, int lineCount) ReadNonStartEndLines(string file)
        {
            var cleanedLines = File.ReadAllLines(file)
                .Where(x => !StartEndTester.IsStartOrEnd(x.TrimStart())).ToList();
            return (string.Join(Environment.NewLine, cleanedLines), cleanedLines.Count);
        }
    }
}