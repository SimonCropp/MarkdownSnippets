using System;
using System.Diagnostics.CodeAnalysis;
using MarkdownSnippets;

static class SnippetKeyReader
{
    public static bool TryExtractKeyFromLine(Line line, [NotNullWhen(true)] out string? key)
    {
        if (!line.Current.StartsWith("snippet:", StringComparison.OrdinalIgnoreCase))
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
}