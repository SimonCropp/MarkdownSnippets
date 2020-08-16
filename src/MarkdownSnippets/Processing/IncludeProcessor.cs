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
        var linesToInject = AddInclude2(line, include,path).ToList();
        var first = linesToInject.First();
        lines[index] = first;

        for (var includeIndex = 1; includeIndex < linesToInject.Count; includeIndex++)
        {
            var lineToInject = linesToInject[includeIndex];
            lines.Insert(index + includeIndex, lineToInject);
        }
    }

    IEnumerable<Line> AddInclude2(Line line, Include include, string? path)
    {
        if (!include.Lines.Any())
        {
            yield return line.WithCurrent($@"<!-- emptyInclude: {include.Key}. path: {path} -->");
            yield break;
        }

        var firstLine = include.Lines.First();

        var linesCount = include.Lines.Count;
        if (SnippetKey.IsSnippetLine(firstLine))
        {
            yield return line.WithCurrent($@"<!-- include: {include.Key}. path: {path} -->");
            yield return new Line(firstLine, include.Path, 1);
            yield return new Line($@"<!-- endInclude: {include.Key}. path: {path} -->", include.Path, 1);

            if (linesCount == 1)
            {
                yield break;
            }
        }
        else
        {
            if (linesCount == 1)
            {
                yield return line.WithCurrent($@"{firstLine} <!-- singleLineInclude: {include.Key}. path: {path} -->");
                yield break;
            }

            yield return line.WithCurrent($@"{firstLine} <!-- include: {include.Key}. path: {path} -->");
        }

        for (var includeIndex = 1; includeIndex < linesCount - 1; includeIndex++)
        {
            var includeLine = include.Lines[includeIndex];
            yield return (new Line(includeLine, include.Path, includeIndex));
        }

        var lastLine = include.Lines.Last();
        if (SnippetKey.IsSnippetLine(lastLine))
        {
            yield return new Line(lastLine, include.Path, linesCount);
            yield return new Line($@"<!-- endInclude: {include.Key}. path: {path} -->", include.Path, linesCount);
        }
        else
        {
            yield return new Line($@"{lastLine} <!-- endInclude: {include.Key}. path: {path} -->", include.Path, linesCount);
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