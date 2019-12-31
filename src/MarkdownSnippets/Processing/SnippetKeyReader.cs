using System;
using System.Diagnostics.CodeAnalysis;
using MarkdownSnippets;

static class SnippetKeyReader
{
    public static bool TryExtractKeyFromLine(Line line, [NotNullWhen(true)] out string? key)
    {
        if (!IsSnippetLine(line))
        {
            key = null;
            return false;
        }

        key = line.Current.Substring(8);

        if (!key.StartsWith(" "))
        {
            throw new MarkdownProcessingException($"Invalid syntax for the snippet '{key}': There must be a space before the start of the key.", line.Path, line.LineNumber);
        }

        key = key.Trim();
        return true;
    }

    public static bool IsSnippetLine(Line line)
    {
        var lineCurrent = line.Current;
        return IsSnippetLine(lineCurrent);
    }

    public static bool IsSnippetLine(string lineCurrent)
    {
        return lineCurrent.StartsWith("snippet:", StringComparison.OrdinalIgnoreCase);
    }
}