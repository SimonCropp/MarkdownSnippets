using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownSnippets;

class IncludeProcessor
{
    DocumentConvention convention;
    IReadOnlyList<Include> includes;
    List<string> allFiles;
    string rootDirectory;

    public IncludeProcessor(
        DocumentConvention convention,
        IReadOnlyList<Include> includes,
        string rootDirectory,
        List<string> allFiles)
    {
        rootDirectory = Path.GetFullPath(rootDirectory);
        this.rootDirectory = rootDirectory.Replace(@"\", "/");
        this.convention = convention;
        this.includes = includes;
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

        string GetIncludeKey(string substring)
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
        var include = includes.SingleOrDefault(x => string.Equals(x.Key, includeKey, StringComparison.OrdinalIgnoreCase));
        if (include != null)
        {
            AddInclude(lines, line, used, index, include, true);
            return;
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

        if (RelativeFile.Find(allFiles, rootDirectory, includeKey, relativePath, line.Path, out var path))
        {
            include = Include.Build(includeKey, File.ReadAllLines(path!), path);
            AddInclude(lines, line, used, index, include, false);
            return;
        }

        missing.Add(new MissingInclude(includeKey, index + 1, line.Path));
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
        if (IsSnippetLineOrEndsWithTicks(first))
        {
            if (writePath)
            {
                yield return line.WithCurrent($"<!-- include: {key}. path: {path} -->");
            }
            else
            {
                yield return line.WithCurrent($"<!-- include: {key} -->");
            }

            yield return new Line(first, path, 1);
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
            yield return new Line(includeLine, path, index);
        }

        var last = include.Lines.Last();
        if (IsSnippetLineOrEndsWithTicks(last))
        {
            yield return new Line(last, path, count);
            yield return new Line("<!-- endInclude -->", path, count);
        }
        else
        {
            yield return new Line($"{last} <!-- endInclude -->", path, count);
        }
    }

    static bool IsSnippetLineOrEndsWithTicks(string line)
    {
        return SnippetKey.IsSnippetLine(line) ||
               line.StartsWith("<!-- endSnippet -->") ||
               line.EndsWith("```");
    }

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
        if (IsSnippetLineOrEndsWithTicks(first))
        {
            if (writePath)
            {
                yield return line.WithCurrent($"<!-- include: {key}. path: {path} -->");
            }
            else
            {
                yield return line.WithCurrent($"<!-- include: {key} -->");
            }

            yield return new Line(first, path, 1);
            yield return new Line("<!-- endInclude -->", path, 1);
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

    string? GetPath(Include include)
    {
        if (include.Path == null)
        {
            return null;
        }

        var path = include.Path.Replace(@"\", "/");
        if (!path.StartsWith(rootDirectory))
        {
            return path;
        }

        return path.Substring(rootDirectory.Length);
    }
}