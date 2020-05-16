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
                include = Include.Build(includeKey, File.ReadAllLines(path), null);
                AddInclude(lines, line, usedIncludes, index, include);
                return;
            }
        }

        missingIncludes.Add(new MissingInclude(includeKey, index + 1, line.Path));
        line.Current = $"** Could not find include '{includeKey}.include.md' **";
    }

    void AddInclude(List<Line> lines, Line line, List<Include> usedIncludes, int index, Include include)
    {
        usedIncludes.Add(include);
        var path = GetPath(include);
        if (!include.Lines.Any())
        {
            line.Current = $@"<!-- include: {include.Key}. path: {path} -->";
            return;
        }

        var firstLine = include.Lines.First();

        var linesCount = include.Lines.Count;
        if (SnippetKeyReader.IsSnippetLine(firstLine))
        {
            line.Current = $@"<!-- include: {include.Key}. path: {path} -->";
            lines.Insert(index + 1, new Line(firstLine, include.Path, 1));
            lines.Insert(index + 2, new Line($@"<!-- end include: {include.Key}. path: {path} -->", include.Path, 1));

            if (linesCount == 1)
            {
                return;
            }
        }
        else
        {
            line.Current = $@"{firstLine} <!-- include: {include.Key}. path: {path} -->";

            if (linesCount == 1)
            {
                return;
            }
        }

        for (var includeIndex = 1; includeIndex < linesCount - 1; includeIndex++)
        {
            var includeLine = include.Lines[includeIndex];

            //todo: path of include
            lines.Insert(index + includeIndex, new Line(includeLine, include.Path, includeIndex));
        }

        var lastLine = include.Lines.Last();
        if (SnippetKeyReader.IsSnippetLine(lastLine))
        {
            lines.Insert(index + linesCount - 1, new Line(lastLine, include.Path, linesCount));
            lines.Insert(index + linesCount, new Line($@"<!-- end include: {include.Key}. path: {path} -->", include.Path, linesCount));
        }
        else
        {
            lines.Insert(index + linesCount - 1, new Line($@"{lastLine} <!-- end include: {include.Key}. path: {path} -->", include.Path, linesCount));
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