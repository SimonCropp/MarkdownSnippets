using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownSnippets;

class IncludeProcessor
{
    IReadOnlyList<Include> includes;
    string rootDirectory;

    public IncludeProcessor(IReadOnlyList<Include> includes, string rootDirectory)
    {
        rootDirectory = Path.GetFullPath(rootDirectory);
        this.rootDirectory = rootDirectory.Replace(@"\", "/");
        this.includes = includes;
    }

    public bool TryProcessInclude(List<Line> lines, Line line, List<Include> used, int index, List<MissingInclude> missing)
    {
        if (!line.Current.StartsWith("include: "))
        {
            return false;
        }

        Inner(lines, line, used, index, missing);

        return true;
    }

    void Inner(List<Line> lines, Line line, List<Include> used, int index, List<MissingInclude> missing)
    {
        var includeKey = line.Current.Substring(9);
        var include = includes.SingleOrDefault(x => string.Equals(x.Key, includeKey, StringComparison.OrdinalIgnoreCase));
        if (include != null)
        {
            AddInclude(lines, line, used, index, include);
            return;
        }

        if (includeKey.StartsWith("http"))
        {
            var (success, path) = Downloader.DownloadFile(includeKey).GetAwaiter().GetResult();
            if (success)
            {
                include = Include.Build(includeKey, File.ReadAllLines(path!), null);
                AddInclude(lines, line, used, index, include);
                return;
            }
        }

        missing.Add(new MissingInclude(includeKey, index + 1, line.Path));
        line.Current = $@"** Could not find include '{includeKey}' ** <!-- singleLineInclude: {includeKey} -->";
    }

    void AddInclude(List<Line> lines, Line line, List<Include> used, int index, Include include)
    {
        used.Add(include);
        var linesToInject = BuildIncludes(line, include).ToList();
        var first = linesToInject.First();
        lines[index] = first;

        for (var includeIndex = 1; includeIndex < linesToInject.Count; includeIndex++)
        {
            var lineToInject = linesToInject[includeIndex];
            lines.Insert(index + includeIndex, lineToInject);
        }
    }

    IEnumerable<Line> BuildIncludes(Line line, Include include)
    {
        var path = GetPath(include);

        var count = include.Lines.Count;
        if (count == 0)
        {
            return BuildEmpty(line, path, include);
        }

        if (count == 1)
        {
            return BuildSingle(line, path, include);
        }

        return BuildMultiple(line, path, include);
    }

    static IEnumerable<Line> BuildMultiple(Line line, string? path, Include include)
    {
        var count = include.Lines.Count;
        var first = include.Lines.First();
        var key = include.Key;
        if (SnippetKey.IsSnippetLine(first))
        {
            yield return line.WithCurrent($@"<!-- include: {key}. path: {path} -->");
            yield return new Line(first, path, 1);
        }
        else
        {
            yield return line.WithCurrent($@"{first} <!-- include: {key}. path: {path} -->");
        }

        for (var index = 1; index < include.Lines.Count - 1; index++)
        {
            var includeLine = include.Lines[index];
            yield return new Line(includeLine, path, index);
        }

        var last = include.Lines.Last();
        if (SnippetKey.IsSnippetLine(last))
        {
            yield return new Line(last, path, count);
            yield return new Line($@"<!-- endInclude: {key}. path: {path} -->", path, count);
        }
        else
        {
            yield return new Line($@"{last} <!-- endInclude: {key}. path: {path} -->", path, count);
        }
    }

    static IEnumerable<Line> BuildEmpty(Line line, string? path, Include include)
    {
        yield return line.WithCurrent($@"<!-- emptyInclude: {include.Key}. path: {path} -->");
    }

    static IEnumerable<Line> BuildSingle(Line line, string? path, Include include)
    {
        var first = include.Lines.First();
        var key = include.Key;
        if (SnippetKey.IsSnippetLine(first))
        {
            yield return line.WithCurrent($@"<!-- include: {key}. path: {path} -->");
            yield return new Line(first, path, 1);
            yield return new Line($@"<!-- endInclude: {key}. path: {path} -->", path, 1);
        }
        else
        {
            yield return line.WithCurrent($@"{first} <!-- singleLineInclude: {key}. path: {path} -->");
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