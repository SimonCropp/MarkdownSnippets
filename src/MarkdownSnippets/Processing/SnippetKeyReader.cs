using System;
using System.Diagnostics.CodeAnalysis;
using MarkdownSnippets;

static class SnippetKeyReader
{
    public static bool TryExtractKeyFromLine(Line line, [NotNullWhen(true)] out string? key)
    {
        var lineCurrent = line.Current;
        if (!IsSnippetLine(lineCurrent))
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
        //TODO: better validation
        //if (KeyValidator.IsInValidKey(key))
        //{
        //    throw new MarkdownProcessingException($@"Invalid syntax for the snippet '{key}': Cannot contain whitespace or start/end with symbols.", line.Path, line.LineNumber);
        //}
        return true;
    }

    public static bool IsSnippetLine(string lineCurrent)
    {
        return lineCurrent.StartsWith("snippet:", StringComparison.OrdinalIgnoreCase);
    }
}