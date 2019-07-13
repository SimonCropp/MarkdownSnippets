using System;
using MarkdownSnippets;

static class StartEndTester
{
    internal static bool IsStartOrEnd(string trimmedLine)
    {
        return IsBeginSnippet(trimmedLine) ||
               IsEndSnippet(trimmedLine) ||
               IsStartCode(trimmedLine) ||
               IsEndCode(trimmedLine) ||
               IsStartRegion(trimmedLine) ||
               IsEndRegion(trimmedLine);
    }

    internal static bool IsStart(string trimmedLine, string path, out string currentKey, out Func<string, bool> endFunc)
    {
        if (IsBeginSnippet(trimmedLine, path, out currentKey))
        {
            endFunc = IsEndSnippet;
            return true;
        }

        if (IsStartCode(trimmedLine, path, out currentKey))
        {
            endFunc = IsEndCode;
            return true;
        }

        if (IsStartRegion(trimmedLine, path, out currentKey))
        {
            endFunc = IsEndRegion;
            return true;
        }

        endFunc = null;
        return false;
    }

    static bool IsEndRegion(string line)
    {
        return line.IndexOf("#endregion", StringComparison.Ordinal) >= 0;
    }

    static bool IsEndCode(string line)
    {
        return line.IndexOf("endcode", StringComparison.Ordinal) >= 0;
    }

    static bool IsEndSnippet(string line)
    {
        return line.IndexOf("end-snippet", StringComparison.Ordinal) >= 0;
    }

    static bool IsStartRegion(string line)
    {
        return line.StartsWith("#region ", StringComparison.Ordinal);
    }

    internal static bool IsStartRegion(string line, string path, out string key)
    {
        if (!line.StartsWith("#region ", StringComparison.Ordinal))
        {
            key = null;
            return false;
        }
        var substring = line.Substring(8);
        return TryExtractParts(substring, line, false, path, out key);
    }

    static bool IsBeginSnippet(string line)
    {
        var startCodeIndex = line.IndexOf("begin-snippet: ", StringComparison.Ordinal);
        return startCodeIndex != -1;
    }

    internal static bool IsBeginSnippet(string line, string path, out string key)
    {
        var startCodeIndex = line.IndexOf("begin-snippet: ", StringComparison.Ordinal);
        if (startCodeIndex == -1)
        {
            key  = null;
            return false;
        }
        var startIndex = startCodeIndex + 15;
        var substring = line
            .TrimBackCommentChars(startIndex);
        return TryExtractParts(substring, line, true, path, out key);
    }

    static bool IsStartCode(string line)
    {
        var startCodeIndex = line.IndexOf("startcode ", StringComparison.Ordinal);
        return startCodeIndex != -1;
    }

    internal static bool IsStartCode(string line, string path, out string key)
    {
        var startCodeIndex = line.IndexOf("startcode ", StringComparison.Ordinal);
        if (startCodeIndex == -1)
        {
            key  = null;
            return false;
        }
        var startIndex = startCodeIndex + 10;
        var substring = line
            .TrimBackCommentChars(startIndex);
        return TryExtractParts(substring, line, true, path, out key);
    }

    static bool TryExtractParts(string substring, string line, bool throwForTooManyParts, string path, out string key)
    {
        var split = substring.SplitBySpace();
        if (split.Length == 0)
        {
            throw new SnippetReadingException($"No Key could be derived. Line: '{line}'.");
        }
        key = split[0];
        KeyValidator.ValidateKeyDoesNotStartOrEndWithSymbol(key, path, line);
        if (split.Length == 1)
        {
            return true;
        }

        if (!throwForTooManyParts)
        {
            return false;
        }
        throw new SnippetReadingException($"Too many parts. Line: '{line}'.");
    }
}