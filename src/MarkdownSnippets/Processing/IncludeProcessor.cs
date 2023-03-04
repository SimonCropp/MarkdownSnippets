using MarkdownSnippets;

class IncludeProcessor
{
    DocumentConvention convention;
    IReadOnlyList<Include> includes;
    IReadOnlyDictionary<string, IReadOnlyList<Snippet>> snippets;
    IReadOnlyList<string> allFiles;
    string targetDirectory;

    public IncludeProcessor(
        DocumentConvention convention,
        IReadOnlyList<Include> includes,
        IReadOnlyDictionary<string, IReadOnlyList<Snippet>> snippets,
        string targetDirectory,
        IReadOnlyList<string> allFiles)
    {
        targetDirectory = Path.GetFullPath(targetDirectory);
        this.targetDirectory = targetDirectory.Replace('\\', '/');
        this.convention = convention;
        this.includes = includes;
        this.snippets = snippets;
        this.allFiles = allFiles;
    }

    public bool TryProcessInclude(List<Line> lines, Line line, List<Include> used, int index, List<MissingInclude> missing, string? relativePath)
    {
        var current = line.Current;

        if (current.StartsWith("include: "))
        {
            var includeKey = line.Current.Substring(9);
            Inner(lines, line, used, index, missing, includeKey, relativePath);
            return true;
        }

        if (convention == DocumentConvention.SourceTransform)
        {
            return false;
        }

        static string GetIncludeKey(string substring)
        {
            var indexOfDotPath = substring.IndexOf(". path:");
            if (indexOfDotPath != -1)
            {
                return substring.Substring(0, indexOfDotPath);
            }

            return substring.Substring(0, substring.Length - 4);
        }

        var indexSingleLineInclude = current.IndexOf("<!-- singleLineInclude: ", StringComparison.Ordinal);
        if (indexSingleLineInclude > 0)
        {
            var substring = current.Substring(indexSingleLineInclude + 24);
            var includeKey = GetIncludeKey(substring);
            Inner(lines, line, used, index, missing, includeKey, relativePath);
            return true;
        }

        if (current.StartsWith("<!-- include: ", StringComparison.Ordinal))
        {
            var substring = current.Substring(14);
            var includeKey = GetIncludeKey(substring);
            lines.RemoveUntil(index + 1, "<!-- endInclude -->", line.Path, line);
            Inner(lines, line, used, index, missing, includeKey, relativePath);
            return true;
        }

        var indexOfInclude = current.IndexOf("<!-- include: ", StringComparison.Ordinal);
        if (indexOfInclude > 0)
        {
            var substring = current.Substring(indexOfInclude + 14);
            var includeKey = GetIncludeKey(substring);
            lines.RemoveUntil(index + 1, "<!-- endInclude -->", line.Path, line);
            Inner(lines, line, used, index, missing, includeKey, relativePath);
            return true;
        }

        return false;
    }

    void Inner(List<Line> lines, Line line, List<Include> used, int index, List<MissingInclude> missing, string includeKey, string? relativePath)
    {
        var include = includes.SingleOrDefault(_ => string.Equals(_.Key, includeKey, StringComparison.OrdinalIgnoreCase));
        if (include != null)
        {
            AddInclude(lines, line, used, index, include, true);
            return;
        }

        if (snippets.TryGetValue(includeKey, out var snippetsResult))
        {
            if (snippetsResult.Count > 1)
            {
                throw new("Only one snippet may be used as an include");
            }

            if (snippetsResult.Count == 1)
            {
                var snippet = snippetsResult[0];
                var snippetInclude = Include.Build(snippet.Key, snippet.Value.Lines(), snippet.Path);
                AddInclude(lines, line, used, index, snippetInclude, true);
                return;
            }
        }

        if (includeKey.StartsWith("http"))
        {
            var (success, httpPath) = Downloader.DownloadFile(includeKey).GetAwaiter().GetResult();
            if (success)
            {
                include = Include.Build(includeKey, File.ReadAllLines(httpPath!), null);
                AddInclude(lines, line, used, index, include, false);
                return;
            }
        }

        if (RelativeFile.Find(allFiles, targetDirectory, includeKey, relativePath, line.Path, out var path))
        {
            include = Include.Build(includeKey, File.ReadAllLines(path), path);
            AddInclude(lines, line, used, index, include, false);
            return;
        }

        missing.Add(new(includeKey, index + 1, line.Path));
        line.Current = $"** Could not find include '{includeKey}' ** <!-- singleLineInclude: {includeKey} -->";
    }

    void AddInclude(List<Line> lines, Line line, List<Include> used, int index, Include include, bool writePath)
    {
        used.Add(include);
        var linesToInject = BuildIncludes(line, include, writePath).ToList();
        var first = linesToInject.First();
        lines[index] = first;

        for (var includeIndex = 1; includeIndex < linesToInject.Count; includeIndex++)
        {
            var lineToInject = linesToInject[includeIndex];
            lines.Insert(index + includeIndex, lineToInject);
        }
    }

    IEnumerable<Line> BuildIncludes(Line line, Include include, bool writePath)
    {
        var path = GetPath(include);

        var count = include.Lines.Count;
        if (count == 0)
        {
            return BuildEmpty(line, path, include, writePath);
        }

        if (count == 1)
        {
            return BuildSingle(line, path, include, writePath);
        }

        return BuildMultiple(line, path, include, writePath);
    }

    static IEnumerable<Line> BuildMultiple(Line line, string? path, Include include, bool writePath)
    {
        var count = include.Lines.Count;
        var first = include.Lines.First();
        var key = include.Key;
        if (ShouldWriteIncludeOnDiffLine(first.AsSpan()))
        {
            if (writePath)
            {
                yield return line.WithCurrent($"<!-- include: {key}. path: {path} -->");
            }
            else
            {
                yield return line.WithCurrent($"<!-- include: {key} -->");
            }

            yield return new(first, path, 1);
        }
        else
        {
            if (writePath)
            {
                yield return line.WithCurrent($"{first} <!-- include: {key}. path: {path} -->");
            }
            else
            {
                yield return line.WithCurrent($"{first} <!-- include: {key} -->");
            }

        }

        for (var index = 1; index < include.Lines.Count - 1; index++)
        {
            var includeLine = include.Lines[index];
            yield return new(includeLine, path, index);
        }

        var last = include.Lines.Last();
        if (ShouldWriteIncludeOnDiffLine(last.AsSpan()))
        {
            yield return new(last, path, count);
            yield return new("<!-- endInclude -->", path, count);
        }
        else
        {
            yield return new($"{last} <!-- endInclude -->", path, count);
        }
    }

    static bool ShouldWriteIncludeOnDiffLine(CharSpan line) =>
        SnippetKey.IsSnippetLine(line) ||
        line.StartsWith("<!-- endSnippet -->".AsSpan()) ||
        line.EndsWith("```".AsSpan()) ||
        line.StartsWith('|') ||
        line.EndsWith('|');

    static IEnumerable<Line> BuildEmpty(Line line, string? path, Include include, bool writePath)
    {
        if (writePath)
        {
            yield return line.WithCurrent($"<!-- emptyInclude: {include.Key}. path: {path} -->");
        }

        yield return line.WithCurrent($"<!-- emptyInclude: {include.Key} -->");
    }

    static IEnumerable<Line> BuildSingle(Line line, string? path, Include include, bool writePath)
    {
        var first = include.Lines.First();
        var key = include.Key;
        if (ShouldWriteIncludeOnDiffLine(first.AsSpan()))
        {
            if (writePath)
            {
                yield return line.WithCurrent($"<!-- include: {key}. path: {path} -->");
            }
            else
            {
                yield return line.WithCurrent($"<!-- include: {key} -->");
            }

            yield return new(first, path, 1);
            yield return new("<!-- endInclude -->", path, 1);
        }
        else
        {
            if (writePath)
            {
                yield return line.WithCurrent($"{first} <!-- singleLineInclude: {key}. path: {path} -->");
            }
            else
            {
                yield return line.WithCurrent($"{first} <!-- singleLineInclude: {key} -->");
            }
        }
    }

    string? GetPath(IContent include)
    {
        if (include.Path == null)
        {
            return null;
        }

        var path = include.Path.Replace('\\', '/');
        if (!path.StartsWith(targetDirectory))
        {
            return path;
        }

        return path.Substring(targetDirectory.Length);
    }
}