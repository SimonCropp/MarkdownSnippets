using System.Diagnostics.CodeAnalysis;

static class SnippetKey
{
    public static bool ExtractStartCommentSnippet(Line line, [NotNullWhen(true)] out string? key)
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

    public static bool ExtractSnippet(Line line, [NotNullWhen(true)] out string? key)
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

    public static bool IsSnippetLine(string lineCurrent) =>
        lineCurrent.StartsWith("snippet:", StringComparison.OrdinalIgnoreCase);

    public static bool IsStartCommentSnippetLine(string lineCurrent) =>
        lineCurrent.StartsWith("<!-- snippet:", StringComparison.OrdinalIgnoreCase);
}