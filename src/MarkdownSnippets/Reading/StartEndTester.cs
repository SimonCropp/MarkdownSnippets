using System.Diagnostics.CodeAnalysis;
using MarkdownSnippets;

static class StartEndTester
{
    internal static bool IsStartOrEnd(string trimmedLine)
    {
        var span = trimmedLine.AsSpan();
        return IsBeginSnippet(trimmedLine) ||
               IsEndSnippet(span) ||
               IsStartRegion(trimmedLine) ||
               IsEndRegion(span);
    }

    internal static bool IsStart(
        string trimmedLine,
        string path,
        [NotNullWhen(true)] out string? currentKey,
        [NotNullWhen(true)] out IsEndSnippet? endFunc)
    {
        var trimmedLineSpan = trimmedLine.AsSpan();
        if (IsBeginSnippet(trimmedLineSpan, path, out currentKey))
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

    static IsEndSnippet throwFunc = _ => throw new("Do not use out func");

    static bool IsEndRegion(CharSpan line) =>
        line.StartsWith("#endregion".AsSpan(), StringComparison.Ordinal);

    static bool IsEndSnippet(CharSpan line) =>
        IndexOf(line, "end-snippet".AsSpan()) >= 0;

    static bool IsStartRegion(string line) =>
        line.StartsWith("#region ", StringComparison.Ordinal);

    internal static bool IsStartRegion(
        string line,
        [NotNullWhen(true)] out string? key)
    {
        var lineAsSpan = line.AsSpan();
        if (!lineAsSpan.StartsWith("#region ".AsSpan()))
        {
            key = null;
            return false;
        }

        var substring = lineAsSpan.Slice(8);
        var split = substring.SplitBySpace();
        if (split.Length == 0)
        {
            key = null;
            return false;
        }

        if (split.Length != 1)
        {
            key = null;
            return false;
        }

        var keySpan = split[0];

        if (KeyValidator.IsInValidKey(keySpan))
        {
            key = null;
            return false;
        }

        key = keySpan.ToString().ToLowerInvariant();
        return true;
    }

    static bool IsBeginSnippet(string line)
    {
        var startIndex = IndexOf(line, "begin-snippet: ");
        return startIndex != -1;
    }

    internal static bool IsBeginSnippet(
        CharSpan line,
        string path,
        [NotNullWhen(true)] out string? key)
    {
        var beginSnippetIndex = IndexOf(line, "begin-snippet: ".AsSpan());
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
Line: '{line.ToString()}'");
        }

        var keySpan = split[0];
        if (split.Length != 1)
        {
            throw new SnippetReadingException($@"Too many parts.
Path: {path}
Line: '{line.ToString()}'");
        }

        if (KeyValidator.IsInValidKey(keySpan))
        {
            throw new SnippetReadingException($@"Key cannot contain whitespace or start/end with symbols.
Key: {keySpan.ToString()}
Path: {path}
Line: {line.ToString()}");
        }

        key = keySpan.ToString().ToLowerInvariant();
        return true;
    }

    static int IndexOf(string line, string value)
    {
        var charactersToScan = Math.Min(line.Length, value.Length + 10);
        return line.IndexOf(value, startIndex: 0, count: charactersToScan, StringComparison.Ordinal);
    }

    static int IndexOf(CharSpan line, CharSpan value)
    {
        var charactersToScan = Math.Min(line.Length, value.Length + 10);
        return line.Slice(0, charactersToScan).IndexOf(value, StringComparison.Ordinal);
    }
}