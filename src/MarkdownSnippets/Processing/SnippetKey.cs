using System.Diagnostics.CodeAnalysis;
using MarkdownSnippets;

static class SnippetKey
{
    public static bool ExtractStartCommentSnippet(Line line, [NotNullWhen(true)] out string? key)
    {
        var lineCurrent = line.Current.AsSpan();
        if (!IsStartCommentSnippetLine(lineCurrent))
        {
            key = null;
            return false;
        }

        var substring = lineCurrent.Slice(14);
        var indexOf = substring.IndexOf("-->".AsSpan());
        key = substring.Slice(0, indexOf).Trim().ToString();
        return true;
    }

    public static bool ExtractSnippet(Line line, [NotNullWhen(true)] out string? key)
    {
        var lineCurrent = line.Current.AsSpan();
        if (!IsSnippetLine(lineCurrent))
        {
            key = null;
            return false;
        }

        key = lineCurrent.Slice(8).Trim().ToString();
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new SnippetException($"Could not parse snippet from: {line.Original}. Path: {line.Path}. Line: {line.LineNumber}");
        }
        return true;
    }

    public static bool IsSnippetLine(CharSpan lineCurrent) =>
        lineCurrent.StartsWith("snippet:".AsSpan());

    public static bool IsStartCommentSnippetLine(CharSpan lineCurrent) =>
        lineCurrent.StartsWith("<!-- snippet:".AsSpan());
}