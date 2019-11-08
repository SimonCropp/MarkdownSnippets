using System;
using System.Diagnostics.CodeAnalysis;
using MarkdownSnippets;

static class StartEndTester
{
    internal static bool IsStartOrEnd(string trimmedLine)
    {
        return IsBeginSnippet(trimmedLine) ||
               IsEndSnippet(trimmedLine) ||
               IsStartRegion(trimmedLine) ||
               IsEndRegion(trimmedLine);
    }

    internal static bool IsStart(
        string trimmedLine,
        string path,
        [NotNullWhen(true)] out string? currentKey,
        [NotNullWhen(true)] out Func<string, bool>? endFunc)
    {
        if (IsBeginSnippet(trimmedLine, path, out currentKey))
        {
            endFunc = IsEndSnippet;
            return true;
        }

        if (IsStartRegion(trimmedLine, path, out currentKey))
        {
            endFunc = IsEndRegion;
            return true;
        }

        endFunc = throwFunc;
        return false;
    }

    static Func<string, bool> throwFunc = s => throw new Exception("Do not use out func");

    static bool IsEndRegion(string line)
    {
        return line.StartsWith("#endregion", StringComparison.Ordinal);
    }

    static bool IsEndSnippet(string line)
    {
        return IndexOf(line, "end-snippet") >= 0;
    }

    static bool IsStartRegion(string line)
    {
        return line.StartsWith("#region ", StringComparison.Ordinal);
    }

    internal static bool IsStartRegion(
        string line,
        string path,
        [NotNullWhen(true)] out string? key)
    {
        if (!line.StartsWith("#region ", StringComparison.Ordinal))
        {
            key = "";
            return false;
        }

        var substring = line.Substring(8);
        return TryExtractParts(substring, line, false, path, out key);
    }

    static bool IsBeginSnippet(string line)
    {
        var startIndex = IndexOf(line, "begin-snippet: ");
        return startIndex != -1;
    }

    internal static bool IsBeginSnippet(
        string line,
        string path,
        [NotNullWhen(true)] out string? key)
    {
        var beginSnippetIndex = IndexOf(line, "begin-snippet: ");
        if (beginSnippetIndex == -1)
        {
            key = null;
            return false;
        }

        var startIndex = beginSnippetIndex + 15;
        var substring = line
            .TrimBackCommentChars(startIndex);
        return TryExtractParts(substring, line, true, path, out key);
    }

    static int IndexOf(string line, string value)
    {
        var charactersToScan = Math.Min(line.Length, value.Length + 10);
        return line.IndexOf(value, startIndex: 0, count: charactersToScan, StringComparison.Ordinal);
    }

    static bool TryExtractParts(
        string substring,
        string line,
        bool throwForTooManyParts,
        string path,
        [NotNullWhen(true)] out string? key)
    {
        var split = substring.SplitBySpace();
        if (split.Length == 0)
        {
            throw new SnippetReadingException($@"No Key could be derived.
Path: {path}
Line: '{line}'");
        }

        key = split[0].ToLowerInvariant();
        KeyValidator.ValidateKeyDoesNotStartOrEndWithSymbol(key, path, line);
        if (split.Length == 1)
        {
            return true;
        }

        if (!throwForTooManyParts)
        {
            return false;
        }

        throw new SnippetReadingException($@"Too many parts.
Path: {path}
Line: '{line}'");
    }
}