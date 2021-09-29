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
        string newLine;
        string? header;
        int tocLevel;
        List<string> tocExcludes;
        List<string> snippetSourceFiles;
        IncludeProcessor includeProcessor;

        static List<string> validationExcludes = new()
        {
            "code_of_conduct",
            ".github",
            "license"
        };

        string targetDirectory;
        IReadOnlyList<string> allFiles;

        public MarkdownProcessor(
            DocumentConvention convention,
            IReadOnlyDictionary<string, IReadOnlyList<Snippet>> snippets,
            IReadOnlyList<Include> includes,
            AppendSnippetsToMarkdown appendSnippets,
            IReadOnlyList<string> snippetSourceFiles,
            IReadOnlyList<string> allFiles,
            int tocLevel,
            bool writeHeader,
            string targetDirectory,
            bool validateContent,
            string? header = null,
            IEnumerable<string>? tocExcludes = null,
            string newLine = "\n")
        {
            Guard.AgainstEmpty(header, nameof(header));
            Guard.AgainstNegativeAndZero(tocLevel, nameof(tocLevel));
            Guard.AgainstNullAndEmpty(targetDirectory, nameof(targetDirectory));

            if (convention == DocumentConvention.InPlaceOverwrite && writeHeader)
            {
                throw new SnippetException("WriteHeader is not allowed with InPlaceOverwrite convention.");
            }

            this.targetDirectory = Path.GetFullPath(targetDirectory).Replace('\\', '/');

            this.allFiles = allFiles
                .Select(x => x.Replace('\\', '/'))
                .ToList();

            this.convention = convention;
            this.snippets = snippets;
            this.appendSnippets = appendSnippets;
            this.writeHeader = writeHeader;
            this.validateContent = validateContent;
            this.newLine = newLine;
            this.header = header;
            this.tocLevel = tocLevel;
            if (tocExcludes == null)
            {
                this.tocExcludes = new();
            }
            else
            {
                this.tocExcludes = tocExcludes.ToList();
            }

            this.snippetSourceFiles = snippetSourceFiles
                .Select(x => x.Replace('\\', '/'))
                .ToList();
            includeProcessor = new(convention, includes, snippets, targetDirectory, this.allFiles);
        }

        public string Apply(string input, string? file = null)
        {
            Guard.AgainstEmpty(file, nameof(file));
            var builder = StringBuilderCache.Acquire();
            try
            {
                using StringReader reader = new(input);
                using StringWriter writer = new(builder);
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
            Guard.AgainstEmpty(file, nameof(file));
            var lines = Lines.ReadAllLines(textReader, null).ToList();
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
            List<MissingSnippet> missingSnippets = new();
            List<ValidationError> validationErrors = new();
            List<MissingInclude> missingIncludes = new();
            List<Snippet> usedSnippets = new();
            List<Include> usedIncludes = new();
            StringBuilder builder = new();
            Line? tocLine = null;

            void AppendLine(string s)
            {
                builder.Append(s);
                builder.Append(newLine);
            }

            List<Line> headerLines = new();
            for (var index = 0; index < lines.Count; index++)
            {
                var line = lines[index];

                if (ValidateContent(relativePath, line, validationErrors))
                {
                    continue;
                }

                if (includeProcessor.TryProcessInclude(lines, line, usedIncludes, index, missingIncludes, relativePath))
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
                lines.Insert(0, new(HeaderWriter.WriteHeader(relativePath!, header, newLine), "", 0));
            }

            if (tocLine != null)
            {
                tocLine.Current = TocBuilder.BuildToc(headerLines, tocLevel, tocExcludes, newLine);
            }

            return new(
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

            MissingSnippet missing = new(key, line.LineNumber, line.Path);
            missings.Add(missing);
            appendLine("```");
            appendLine($"** Could not find snippet '{key}' **");
            appendLine("```");
            appendLine("<!-- endSnippet -->");
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
            var keyWithDirChar = FileEx.PrependSlash(key);

            snippetsForKey = snippetSourceFiles
                .Where(file => file.EndsWith(keyWithDirChar, StringComparison.OrdinalIgnoreCase))
                .Select(file => FileToSnippet(key, file, file))
                .ToList();
            if (snippetsForKey.Any())
            {
                return true;
            }

            if (RelativeFile.Find(allFiles, targetDirectory, key, relativePath, linePath, out var path))
            {
                snippetsForKey = SnippetsForFile(key, path);
                return true;
            }

            snippetsForKey = null!;
            return false;
        }


        List<Snippet> SnippetsForFile(string key, string relativeToRoot)
        {
            return new()
            {
                FileToSnippet(key, relativeToRoot, null)
            };
        }

        bool GetForHttp(string key, out IReadOnlyList<Snippet> snippetsForKey)
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

        Snippet FileToSnippet(string key, string file, string? path)
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

        (string text, int lineCount) ReadNonStartEndLines(string file)
        {
            var cleanedLines = File.ReadAllLines(file)
                .Where(x => !StartEndTester.IsStartOrEnd(x.TrimStart())).ToList();
            var text = string.Join(newLine, cleanedLines).Trim();
            return (text, cleanedLines.Count);
        }
    }
}