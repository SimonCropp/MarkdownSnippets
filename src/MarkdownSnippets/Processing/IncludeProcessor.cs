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
        if (include == null)
        {
            missingIncludes.Add(new MissingInclude(includeKey, index, line.Path));
            line.Current = $"** Could not find include '{includeKey}.include.md' **";
            return;
        }

        usedIncludes.Add(include);
        var path = GetPath(include);
        line.Current = $@"<!--
{line.Current}
path: {path}
-->";
        for (var includeIndex = 0; includeIndex < include.Lines.Count; includeIndex++)
        {
            var includeLine = include.Lines[includeIndex];

            //todo: path of include
            lines.Insert(index + includeIndex + 1, new Line(includeLine, include.Path, includeIndex));
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