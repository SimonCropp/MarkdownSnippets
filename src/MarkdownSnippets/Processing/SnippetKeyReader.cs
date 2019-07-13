using System;
using MarkdownSnippets;

static class SnippetKeyReader
{
    public static bool TryExtractKeyFromLine(string line, string file, int lineNumber, out string key)
    {
        if (!line.StartsWith("snippet:", StringComparison.OrdinalIgnoreCase))
        {
            key = null;
            return false;
        }

        key = line.Substring(8);

        if (!key.StartsWith(" "))
        {
            throw new MarkdownProcessingException($"Invalid syntax for the snippet '{key}': There must be a space before the start of the key.", file, lineNumber);
        }

        key = key.Trim();
        return true;
    }
}