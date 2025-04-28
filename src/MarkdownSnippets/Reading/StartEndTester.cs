public delegate bool EndFunc(string line);

static partial class StartEndTester
{
    static readonly Regex keyPatternRegex = new(@"([a-zA-Z0-9\-_]+)(?:\((.*?)\))?");

    internal static bool IsStartOrEnd(string trimmedLine) =>
        IsBeginSnippet(trimmedLine) ||
        IsEndSnippet(trimmedLine) ||
        IsStartRegion(trimmedLine) ||
        IsEndRegion(trimmedLine);

    internal static bool IsStart(
        string trimmedLine,
        string path,
        [NotNullWhen(true)] out string? currentKey,
        [NotNullWhen(true)] out EndFunc? endFunc,
        out string? expressiveCode
    )
    {
        if (IsBeginSnippet(trimmedLine, path, out currentKey, out var block))
        {
            endFunc = IsEndSnippet;
            expressiveCode = block;
            return true;
        }

        if (IsStartRegion(trimmedLine, out currentKey))
        {
            endFunc = IsEndRegion;
            // not supported for regions
            expressiveCode = null;
            return true;
        }

        expressiveCode = null;
        endFunc = throwFunc;

        return false;
    }

    static EndFunc throwFunc = _ => throw new("Do not use out func");

    static bool IsEndRegion(string line) =>
        line.StartsWith("#endregion", StringComparison.Ordinal);

    static bool IsEndSnippet(string line) =>
        IndexOf(line, "end-snippet") >= 0;

    static bool IsStartRegion(string line) =>
        line.StartsWith("#region ", StringComparison.Ordinal);

    internal static bool IsStartRegion(
        string line,
        [NotNullWhen(true)] out string? key)
    {
        if (!line.StartsWith("#region ", StringComparison.Ordinal))
        {
            key = null;
            return false;
        }

        var substring = line[8..].Trim();

        if (substring.Contains(' '))
        {
            key = null;
            return false;
        }

        if (!KeyValidator.IsValidKey(substring.AsSpan()))
        {
            key = null;
            return false;
        }

        key = substring;
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
        [NotNullWhen(true)] out string? key,
        [NotNullWhen(true)] out string? expressiveCode
    )
    {
        expressiveCode = null;
        var beginSnippetIndex = IndexOf(line, "begin-snippet: ");
        if (beginSnippetIndex == -1)
        {
            key = null;
            return false;
        }

        var startIndex = beginSnippetIndex + 15;
        var substring = line
            .TrimBackCommentChars(startIndex);

        var match = keyPatternRegex.Match(substring);

        if (match.Length == 0)
        {
            throw new SnippetReadingException(
                $"""
                 No Key could be derived.
                 Path: {path}
                 Line: '{line}'
                 """);
        }

        var partOne = match.Groups[1].Value;
        var split = partOne.SplitBySpace();
        if (split.Length != 1)
        {
            throw new SnippetReadingException(
                $"""
                 Too many parts.
                 Path: {path}
                 Line: '{line}'
                 """);
        }

        key = split[0];
        expressiveCode = match.Groups[2].Value;

        if (KeyValidator.IsValidKey(key.AsSpan()))
        {
            return true;
        }

        throw new SnippetReadingException(
            $"""
             Key cannot contain whitespace or start/end with symbols.
             Key: {key}
             Path: {path}
             Line: {line}
             """);
    }

    static int IndexOf(string line, string value)
    {
        if (value.Length > line.Length)
        {
            return -1;
        }

        var charactersToScan = Math.Min(line.Length, value.Length + 10);
        return line.IndexOf(value, startIndex: 0, count: charactersToScan, StringComparison.Ordinal);
    }
}