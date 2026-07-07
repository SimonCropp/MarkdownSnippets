namespace MarkdownSnippets;

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

    // snippetSourceFiles grouped by their last path segment (case-insensitive) so
    // FilesToSnippets can do an O(1) lookup instead of an EndsWith scan of every file.
    Dictionary<string, List<string>> snippetSourceFilesByName;
    IncludeProcessor includeProcessor;

    static List<string> validationExcludes =
    [
        "code_of_conduct",
        ".github",
        "license"
    ];

    string targetDirectory;
    IReadOnlyList<string> allFiles;

    // Cache for ReadNonStartEndLines results, keyed by file path. A single MarkdownProcessor
    // typically processes many markdown files in one run, and the same whole-file snippet is
    // often referenced from multiple of them; caching avoids re-reading and re-scanning the
    // file every time FileToSnippet is called.
    Dictionary<string, (string text, int lineCount)> readNonStartEndCache = new(StringComparer.Ordinal);

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
            .Select(_ => _.Replace('\\', '/'))
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
            this.tocExcludes = [];
        }
        else
        {
            this.tocExcludes = tocExcludes.ToList();
        }

        var snippetSourceFiles1 = snippetSourceFiles
            .Select(_ => _.Replace('\\', '/'))
            .ToList();
        snippetSourceFilesByName = new(StringComparer.OrdinalIgnoreCase);
        foreach (var file in snippetSourceFiles1)
        {
            var fileName = Path.GetFileName(file);
            if (!snippetSourceFilesByName.TryGetValue(fileName, out var bucket))
            {
                bucket = [];
                snippetSourceFilesByName[fileName] = bucket;
            }
            bucket.Add(file);
        }
        includeProcessor = new(convention, includes, snippets, targetDirectory, this.allFiles);
    }

    public string Apply(string input, string? file = null)
    {
        Guard.AgainstEmpty(file, nameof(file));
        var builder = StringBuilderCache.Acquire();
        try
        {
            using var reader = new StringReader(input);
            using var writer = new StringWriter(builder);
            var processResult = Apply(reader, writer, file);
            var missing = processResult.MissingSnippets;
            if (missing.Count != 0)
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

        for (var index = 0; index < lines.Count - 1; index++)
        {
            var line = lines[index];
            writer.WriteLine(line.Current);
        }

        writer.Write(lines.Last().Current);

        return result;
    }

    internal ProcessResult Apply(List<Line> lines, string newLine, string? relativePath)
    {
        var missingSnippets = new List<MissingSnippet>();
        var validationErrors = new List<ValidationError>();
        var missingIncludes = new List<MissingInclude>();
        var usedSnippets = new HashSet<Snippet>();
        var usedIncludes = new HashSet<Include>();
        var builder = new StringBuilder();
        // The appender wraps builder + newLine + a mutable Indent and exposes a single cached
        // Action<string> delegate. Previously every snippet line allocated a closure object
        // (plus a delegate) via CreateIndentedAppendLine(indent); the appender amortises that
        // to one allocation per Apply call.
        var appender = new IndentedAppender(builder, newLine);
        Line? tocLine = null;

        // The exclusion check depends only on relativePath, which is constant for this Apply call.
        // Hoisting it out of the per-line loop avoids repeated string.Contains calls and the
        // method-group delegate allocation that LINQ Any(method-group) would do on every line.
        var runValidation = validateContent &&
            (relativePath == null || !IsValidationExcluded(relativePath));

        var headerLines = new List<Line>();
        for (var index = 0; index < lines.Count; index++)
        {
            var line = lines[index];

            if (runValidation && ValidateLine(line, validationErrors))
            {
                continue;
            }

            if (includeProcessor.TryProcessInclude(lines, line, usedIncludes, index, missingIncludes, relativePath))
            {
                continue;
            }

            if (line.Current.StartsWith('#'))
            {
                if (tocLine != null)
                {
                    headerLines.Add(line);
                }

                continue;
            }

            if (line.Current.AsSpan().TrimStart().SequenceEqual("toc"))
            {
                tocLine = line;
                continue;
            }

            void AppendSnippet(string key1)
            {
                builder.Clear();
                appender.Indent = line.LeadingWhitespace;
                ProcessSnippetLine(appender.Action, missingSnippets, usedSnippets, key1, relativePath, line);
                builder.TrimEnd();
                line.Current = builder.ToString();
            }

            void AppendWebSnippet(string url, string snippetKey, string? viewUrl = null)
            {
                builder.Clear();
                appender.Indent = line.LeadingWhitespace;
                ProcessWebSnippetLine(appender.Action, missingSnippets, usedSnippets, url, snippetKey, viewUrl, line);
                builder.TrimEnd();
                line.Current = builder.ToString();
            }

            if (SnippetKey.ExtractSnippet(line, out var key))
            {
                AppendSnippet(key);
                continue;
            }

            if (SnippetKey.ExtractWebSnippet(line, out var url, out var snippetKey, out var viewUrl))
            {
                AppendWebSnippet(url, snippetKey, viewUrl);
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

            if (SnippetKey.ExtractStartCommentWebSnippet(line, out url, out snippetKey, out viewUrl))
            {
                AppendWebSnippet(url, snippetKey, viewUrl);

                index++;

                lines.RemoveUntil(
                    index,
                    "<!-- endSnippet -->",
                    relativePath,
                    line);
                continue;
            }

            if (line.Current.AsSpan().TrimStart().SequenceEqual("<!-- toc -->"))
            {
                tocLine = line;

                index++;

                lines.RemoveUntil(index, "<!-- endToc -->", relativePath, line);
            }
        }

        if (writeHeader)
        {
            lines.Insert(0, new(HeaderWriter.WriteHeader(relativePath!, header, newLine), "", 0));
        }

        tocLine?.Current = TocBuilder.BuildToc(headerLines, tocLevel, tocExcludes, newLine);

        return new(
            missingSnippets: missingSnippets,
            usedSnippets: usedSnippets.ToList(),
            usedIncludes: usedIncludes.ToList(),
            missingIncludes: missingIncludes,
            validationErrors: validationErrors);
    }

    static bool IsValidationExcluded(string relativePath)
    {
        foreach (var exclude in validationExcludes)
        {
            if (relativePath.Contains(exclude))
            {
                return true;
            }
        }

        return false;
    }

    static bool ValidateLine(Line line, List<ValidationError> validationErrors)
    {
        var found = false;
        foreach (var error in ContentValidation.Verify(line.Original))
        {
            validationErrors.Add(new(error.error, line.LineNumber, error.column, line.Path));
            found = true;
        }

        return found;
    }

    void ProcessSnippetLine(Action<string> appendLine, List<MissingSnippet> missings, HashSet<Snippet> used, string key, string? relativePath, Line line)
    {
        appendLine($"<!-- snippet: {key} -->");

        if (TryGetSnippets(key, relativePath, line.Path, out var snippetsForKey))
        {
            appendSnippets(key, snippetsForKey, appendLine);
            appendLine("<!-- endSnippet -->");
            used.UnionWith(snippetsForKey);
            return;
        }

        var missing = new MissingSnippet(key, line.LineNumber, line.Path);
        missings.Add(missing);
        appendLine("```");
        appendLine($"** Could not find snippet '{key}' **");
        appendLine("```");
        appendLine("<!-- endSnippet -->");
    }

    void ProcessWebSnippetLine(Action<string> appendLine, List<MissingSnippet> missings, HashSet<Snippet> used, string url, string snippetKey, string? viewUrl, Line line)
    {
        var commentText = viewUrl == null
            ? $"<!-- web-snippet: {url}#{snippetKey} -->"
            : $"<!-- web-snippet: {url}#{snippetKey} {viewUrl} -->";
        appendLine(commentText);
        // Download file content
        try
        {
            var (success, content) = Downloader.DownloadContent(url).GetAwaiter().GetResult();
            if (!success || string.IsNullOrWhiteSpace(content))
            {
                var missing = new MissingSnippet($"{url}#{snippetKey}", line.LineNumber, line.Path);
                missings.Add(missing);
                appendLine("```");
                appendLine($"** Could not fetch or parse web-snippet '{url}#{snippetKey}' **");
                appendLine("```");
                appendLine("<!-- endSnippet -->");
                return;
            }
            // Extract snippets from content
            using var reader = new StringReader(content);
            var snippets = FileSnippetExtractor.Read(reader, url);
            var found = snippets.FirstOrDefault(_ => _.Key == snippetKey);
            if (found == null)
            {
                var missing = new MissingSnippet($"{url}#{snippetKey}", line.LineNumber, line.Path);
                missings.Add(missing);
                appendLine("```");
                appendLine($"** Could not find snippet '{snippetKey}' in '{url}' **");
                appendLine("```");
                appendLine("<!-- endSnippet -->");
                return;
            }
            // Create new snippet with viewUrl if provided
            var snippetToAppend = viewUrl == null
                ? found
                : Snippet.Build(
                    language: found.Language,
                    startLine: found.StartLine,
                    endLine: found.EndLine,
                    value: found.Value,
                    key: found.Key,
                    path: found.Path,
                    expressiveCode: found.ExpressiveCode,
                    viewUrl: viewUrl);
            appendSnippets(snippetKey, [snippetToAppend], appendLine);
            appendLine("<!-- endSnippet -->");
            used.Add(snippetToAppend);
        }
        catch
        {
            var missing = new MissingSnippet($"{url}#{snippetKey}", line.LineNumber, line.Path);
            missings.Add(missing);
            appendLine("```");
            appendLine($"** Could not fetch or parse web-snippet '{url}#{snippetKey}' **");
            appendLine("```");
            appendLine("<!-- endSnippet -->");
        }
    }

    bool TryGetSnippets(
        string key,
        string? relativePath,
        string? linePath,
        [NotNullWhen(true)] out IReadOnlyList<Snippet>? snippetsForKey)
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

    bool FilesToSnippets(
        string key,
        string? relativePath,
        string? linePath,
        [NotNullWhen(true)] out IReadOnlyList<Snippet>? snippetsForKey)
    {
        // The key may be a multi-segment path (e.g. "Dir/File.cs"); look up by its last segment
        // via the prebuilt dictionary, then verify each candidate ends with "/key".
        var keyName = Path.GetFileName(key);
        if (keyName.Length != 0 &&
            snippetSourceFilesByName.TryGetValue(keyName, out var bucket))
        {
            var keyWithDirChar = FileEx.PrependSlash(key);
            List<Snippet>? matches = null;
            foreach (var candidate in bucket)
            {
                if (candidate.EndsWith(keyWithDirChar, StringComparison.OrdinalIgnoreCase))
                {
                    matches ??= [];
                    matches.Add(FileToSnippet(key, candidate, candidate));
                }
            }

            if (matches != null)
            {
                snippetsForKey = matches;
                return true;
            }
        }

        if (RelativeFile.Find(allFiles, targetDirectory, key, relativePath, linePath, out var path))
        {
            snippetsForKey = SnippetsForFile(key, path);
            return true;
        }

        snippetsForKey = null!;
        return false;
    }


    List<Snippet> SnippetsForFile(string key, string relativeToRoot) =>
        [FileToSnippet(key, relativeToRoot, null)];

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
            language: FileSnippetExtractor.GetLanguageFromPath(file),
            path: path,
            expressiveCode: null);
    }

    (string text, int lineCount) ReadNonStartEndLines(string file)
    {
        if (readNonStartEndCache.TryGetValue(file, out var cached))
        {
            return cached;
        }

        var builder = StringBuilderCache.Acquire();
        try
        {
            var lineCount = 0;
            foreach (var line in File.ReadLines(file))
            {
                if (!StartEndTester.IsStartOrEnd(line.AsSpan().TrimStart()))
                {
                    if (lineCount > 0)
                    {
                        builder.Append(newLine);
                    }

                    builder.Append(line);
                    lineCount++;
                }
            }

            builder.TrimEnd();
            var start = 0;
            while (start < builder.Length && char.IsWhiteSpace(builder[start]))
            {
                start++;
            }

            var result = (builder.ToString(start, builder.Length - start), lineCount);
            readNonStartEndCache[file] = result;
            return result;
        }
        finally
        {
            StringBuilderCache.Release(builder);
        }
    }

    sealed class IndentedAppender
    {
        readonly StringBuilder builder;
        readonly string newLine;

        public IndentedAppender(StringBuilder builder, string newLine)
        {
            this.builder = builder;
            this.newLine = newLine;
            Action = AppendImpl;
        }

        public string Indent { get; set; } = "";

        // Cached delegate. Callers pass Action to APIs that expect Action<string>; without
        // this field every reference to a method group would allocate a fresh delegate.
        public Action<string> Action { get; }

        void AppendImpl(string s)
        {
            var first = true;
            foreach (var line in s.AsSpan().EnumerateLines())
            {
                if (!first)
                {
                    builder.Append(newLine);
                }
                first = false;
                builder.Append(Indent);
                builder.Append(line);
            }
            builder.Append(newLine);
        }
    }
}
