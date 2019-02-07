using System;
using CaptureSnippets;

static class StartEndTester
{
    static Func<string, bool> isEndCode = s => IsEndCode(s);
    static Func<string, bool> isEndRegion = s => IsEndRegion(s);

    internal static bool IsStart(string trimmedLine, out string currentKey, out Func<string, bool> endFunc)
    {
        if (IsStartCode(trimmedLine, out currentKey))
        {
            endFunc = isEndCode;
            return true;
        }
        if (IsStartRegion(trimmedLine, out currentKey))
        {
            endFunc = isEndRegion;
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

    internal static bool IsStartRegion(string line, out string key)
    {
        if (!line.StartsWith("#region ", StringComparison.Ordinal))
        {
            key = null;
            return false;
        }
        var substring = line.Substring(8);
        return TryExtractParts(out key, substring, line, false);
    }

    internal static bool IsStartCode(string line, out string key)
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
        return TryExtractParts(out key, substring, line, true);
    }

    static bool TryExtractParts(out string key, string substring, string line, bool throwForTooManyParts)
    {
        var split = substring.SplitBySpace();
        if (split.Length == 0)
        {
            throw new SnippetReadingException($"No Key could be derived. Line: '{line}'.");
        }
        key = split[0];
        KeyValidator.ValidateKeyDoesNotStartOrEndWithSymbol(key);
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