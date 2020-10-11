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

        if (IsStartRegion(trimmedLine, out currentKey))
        {
            endFunc = IsEndRegion;
            return true;
        }

        endFunc = throwFunc;
        return false;
    }

    static Func<string, bool> throwFunc = _ => throw new Exception("Do not use out func");

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
        [NotNullWhen(true)] out string? key)
    {
        if (!line.StartsWith("#region ", StringComparison.Ordinal))
        {
            key = "";
            return false;
        }

        var substring = line.Substring(8);
        var split = substring.SplitBySpace();
        if (split.Length == 0)
        {
            key = "";
            return false;
        }

        key = split[0].ToLowerInvariant();
        if (split.Length != 1)
        {
            return false;
        }

        if (KeyValidator.IsInValidKey(key))
        {
            return false;
        }

        return true;
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
        var split = substring.SplitBySpace();
        if (split.Length == 0)
        {
            throw new SnippetReadingException($@"No Key could be derived.
Path: {path}
Line: '{line}'");
        }

        key = split[0].ToLowerInvariant();
        if (split.Length != 1)
        {
            throw new SnippetReadingException($@"Too many parts.
Path: {path}
Line: '{line}'");
        }

        if (KeyValidator.IsInValidKey(key))
        {
            throw new SnippetReadingException($@"Key cannot contain whitespace or start/end with symbols.
Key: {key}
Path: {path}
Line: {line}");
        }

        return true;
    }

    static int IndexOf(string line, string value)
    {
        var charactersToScan = Math.Min(line.Length, value.Length + 10);
        return line.IndexOf(value, startIndex: 0, count: charactersToScan, StringComparison.Ordinal);
    }
}