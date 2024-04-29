using System.Diagnostics.CodeAnalysis;

class SnippetKey
{
    public bool ExtractStartCommentSnippet(Line line, [NotNullWhen(true)] out string? key)
    {
        var lineCurrent = line.Current;
        if (!IsStartCommentSnippetLine(lineCurrent))
        {
            key = null;
            return false;
        }

        var substring = line.Current[14..];
        var indexOf = substring.IndexOf("-->");
        key = substring[..indexOf]
            .Trim();
        return true;
    }

    public bool ExtractSnippet(Line line, [NotNullWhen(true)] out string? key)
    {
        var lineCurrent = line.Current;
        if (!IsSnippetLine(lineCurrent))
        {
            key = null;
            return false;
        }

        key = line.Current[8..]
            .Trim();
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new SnippetException($"Could not parse snippet from: {line.Original}. Path: {line.Path}. Line: {line.LineNumber}");
        }
        return true;
    }

    public bool IsSnippetLine(string lineCurrent) =>
        lineCurrent.StartsWith("snippet:", StringComparison.OrdinalIgnoreCase);

    public bool IsStartCommentSnippetLine(string lineCurrent) =>
        lineCurrent.StartsWith("<!-- snippet:", StringComparison.OrdinalIgnoreCase);

    public static int HandleInPlaceSnippet(MarkdownProcessor markdownProcessor, List<Line> lines, string? relativePath, StringBuilder builder, int index, List<MissingSnippet> missingSnippets, List<Snippet> usedSnippets, Action<string> appendLine, string key, Line line)
    {
        builder.Clear();
        markdownProcessor.ProcessSnippetLine(appendLine, missingSnippets, usedSnippets, key, relativePath, line);
        builder.TrimEnd();
        line.Current = builder.ToString();

        index++;

        lines.RemoveUntil(
            index,
            "<!-- endSnippet -->",
            relativePath,
            line);
        return index;
    }

    public static int HandleSourceTransform(MarkdownProcessor markdownProcessor, string? relativePath, StringBuilder builder, int index, List<MissingSnippet> missingSnippets, List<Snippet> usedSnippets, Action<string> appendLine, string key, Line line)
    {
        builder.Clear();
        markdownProcessor.ProcessSnippetLine(appendLine, missingSnippets, usedSnippets, key, relativePath, line);
        builder.TrimEnd();
        line.Current = builder.ToString();
        return index;
    }
}