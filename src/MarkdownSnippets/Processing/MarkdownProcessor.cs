using System.Diagnostics.CodeAnalysis;

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
    List<string> snippetSourceFiles;
    IncludeProcessor includeProcessor;
    IReadOnlyList<ISnippet> snippetKeys = [new CodeSnippet()];

    static List<string> validationExcludes =
    [
        "code_of_conduct",
        ".github",
        "license"
    ];

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

        this.snippetSourceFiles = snippetSourceFiles
            .Select(_ => _.Replace('\\', '/'))
            .ToList();
        includeProcessor = new(convention, includes, snippets, targetDirectory, this.allFiles, snippetKeys);
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
        var usedSnippets = new List<Snippet>();
        var usedIncludes = new List<Include>();
        var builder = new StringBuilder();
        Line? tocLine = null;


        var headerLines = new List<Line>();
        for (var index = 0; index < lines.Count; index++)
        {
            index = ProcessLine(lines, newLine, relativePath, builder, index, validationErrors, usedIncludes, missingIncludes, headerLines, missingSnippets, usedSnippets, ref tocLine);
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

    int ProcessLine(List<Line> lines, string newLine, string? relativePath, StringBuilder builder, int index, List<ValidationError> validationErrors, List<Include> usedIncludes, List<MissingInclude> missingIncludes, List<Line> headerLines, List<MissingSnippet> missingSnippets, List<Snippet> usedSnippets, ref Line? tocLine)
    {
        Action<string> appendLine = s =>
        {
            builder.Append(s);
            builder.Append(newLine);
        };
        var line = lines[index];


        foreach (var snippetKey in snippetKeys)
        {
            if (ValidateContent(relativePath, line, validationErrors))
            {
                return index;
            }

            if (includeProcessor.TryProcessInclude(lines, line, usedIncludes, index, missingIncludes, relativePath))
            {
                return index;
            }

            if (line.Current.StartsWith('#'))
            {
                if (tocLine != null)
                {
                    headerLines.Add(line);
                }

                return index;
            }

            if (line.Current == "toc")
            {
                tocLine = line;
                return index;
            }

            if (snippetKey.GetNew.ExtractSnippet(line, out var key))
            {
                line.Current = snippetKey.GetNew.Handle(this, lines, relativePath, builder, index, missingSnippets, usedSnippets, appendLine, key, line);
                return index;
            }

            if (convention == DocumentConvention.SourceTransform)
            {
                return index;
            }

            if (snippetKey.GetReplace.ExtractSnippet(line, out key))
            {
                line.Current = snippetKey.GetReplace.Handle(this, lines, relativePath, builder, index, missingSnippets, usedSnippets, appendLine, key, line);
                return index;
            }

            if (line.Current == "<!-- toc -->")
            {
                tocLine = line;

                index++;

                lines.RemoveUntil(index, "<!-- endToc -->", relativePath, line);
            }
        }

        return index;
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
        if (errors.Count == 0)
        {
            return false;
        }

        validationErrors.AddRange(errors.Select(error => new ValidationError(error.error, line.LineNumber, error.column, line.Path)));
        return true;
    }

    public string ProcessSnippetLine(Action<string> appendLine, List<MissingSnippet> missings, List<Snippet> used, string key, string? relativePath, Line line)
    {
        var builder = new StringBuilder();
        builder.Append($"<!-- snippet: {key} -->\n");

        if (TryGetSnippets(key, relativePath, line.Path, out var snippetsForKey))
        {
            builder.Append(appendSnippets(key, snippetsForKey));
            builder.Append("<!-- endSnippet -->\n");
            used.AddRange(snippetsForKey);
        }

        else
        {

            var missing = new MissingSnippet(key, line.LineNumber, line.Path);
            missings.Add(missing);
            builder.Append("```\n");
            builder.Append($"** Could not find snippet '{key}' **\n");
            builder.Append("```\n");
            builder.Append("<!-- endSnippet -->\n");
        }
        return builder.ToString();
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
            language: Path.GetExtension(file)[1..],
            path: path);
    }

    (string text, int lineCount) ReadNonStartEndLines(string file)
    {
        var cleanedLines = File.ReadAllLines(file)
            .Where(_ => !StartEndTester.IsStartOrEnd(_.TrimStart())).ToList();
        var text = string.Join(newLine, cleanedLines).Trim();
        return (text, cleanedLines.Count);
    }
}
