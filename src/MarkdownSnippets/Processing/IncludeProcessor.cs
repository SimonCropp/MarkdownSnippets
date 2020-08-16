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

    public bool TryProcessInclude(List<Line> lines, Line line, List<Include> usedIncludes, int index, List<MissingInclude> missingIncludes)
    {
        if (!line.Current.StartsWith("include: "))
        {
            return false;
        }

        Inner(lines, line, usedIncludes, index, missingIncludes);

        return true;
    }

    void Inner(List<Line> lines, Line line, List<Include> usedIncludes, int index, List<MissingInclude> missingIncludes)
    {
        var includeKey = line.Current.Substring(9);
        var include = includes.SingleOrDefault(x => string.Equals(x.Key, includeKey, StringComparison.OrdinalIgnoreCase));
        if (include != null)
        {
            AddInclude(lines, line, usedIncludes, index, include);
            return;
        }

        if (includeKey.StartsWith("http"))
        {
            var (success, path) = Downloader.DownloadFile(includeKey).GetAwaiter().GetResult();
            if (success)
            {
                include = Include.Build(includeKey, File.ReadAllLines(path!), null);
                AddInclude(lines, line, usedIncludes, index, include);
                return;
            }
        }

        missingIncludes.Add(new MissingInclude(includeKey, index + 1, line.Path));
        line.Current = $@"** Could not find include '{includeKey}' ** <!-- singleLineInclude: {includeKey} -->";
    }

    void AddInclude(List<Line> lines, Line line, List<Include> usedIncludes, int index, Include include)
    {
        usedIncludes.Add(include);
        var path = GetPath(include);
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
        var key = include.Key;
        if (!include.Lines.Any())
        {
            yield return line.WithCurrent($@"<!-- emptyInclude: {key}. path: {path} -->");
            yield break;
        }

        var count = include.Lines.Count;
        var first = include.Lines.First();
        var end = $@"<!-- endInclude: {key}. path: {path} -->";
        var start = $@"<!-- include: {key}. path: {path} -->";
        if (count == 1)
        {
            if (SnippetKey.IsSnippetLine(first))
            {
                yield return line.WithCurrent(start);
                yield return new Line(first, path, 1);
                yield return new Line(end, path, 1);
            }
            else
            {
                yield return line.WithCurrent($@"{first} <!-- singleLineInclude: {key}. path: {path} -->");
            }

            yield break;
        }


        if (SnippetKey.IsSnippetLine(first))
        {
            yield return line.WithCurrent(start);
            yield return new Line(first, path, 1);
        }
        else
        {
            yield return line.WithCurrent($@"{first} {start}");
        }

        for (var index = 1; index < count - 1; index++)
        {
            var includeLine = include.Lines[index];
            yield return new Line(includeLine, path, index);
        }

        var last = include.Lines.Last();
        if (SnippetKey.IsSnippetLine(last))
        {
            yield return new Line(last, path, count);
            yield return new Line(end, path, count);
        }
        else
        {
            yield return new Line($@"{last} {end}", path, count);
        }
    }

    string? GetPath(Include include)
    {
        if (include.Path == null)
        {
            return null;
        }

        var path = include.Path.Replace(@"\", "/");
        if (path.StartsWith(rootDirectory))
        {
            path = path.Substring(rootDirectory.Length);
        }

        return path;
    }
}