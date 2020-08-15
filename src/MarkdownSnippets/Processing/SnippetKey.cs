using System;
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

        var substring = line.Current
            .Substring(14);
        var indexOf = substring.IndexOf("-->");
        key = substring.Substring(0,indexOf)
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

        key = line.Current
            .Substring(8)
            .Trim();
        return true;
    }

    public static bool IsSnippetLine(string lineCurrent)
    {
        return lineCurrent.StartsWith("snippet:", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsStartCommentSnippetLine(string lineCurrent)
    {
        return lineCurrent.StartsWith("<!-- snippet:", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsEndCommentSnippetLine(string lineCurrent)
    {
        return lineCurrent == "<!-- endsnippet -->";
    }
}