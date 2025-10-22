// ReSharper disable PartialTypeWithSinglePart
static partial class StartEndTester
{
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
        out string? expressiveCode)
    {
        if (IsBeginSnippet(trimmedLine, path, out currentKey, out expressiveCode))
        {
            endFunc = IsEndSnippet;
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
        out string? expressiveCode)
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

        var startArgs = substring.IndexOf('(');
        if (startArgs == -1)
        {
            key = substring.Trim();
        }
        else
        {
            substring = substring.Trim();
            key = substring[..startArgs].Trim();

            if (!substring.EndsWith(')'))
            {
                throw new SnippetReadingException(
                    $"""
                     ExpressiveCode must end with ')`.
                     Key: {key}
                     Path: {path}
                     Line: {line}
                     """);
            }

            expressiveCode = substring[(startArgs +1)..^1].Trim();
            if (expressiveCode.Length == 0)
            {
                expressiveCode = null;
            }
        }

        if (key.Length == 0)
        {
            throw new SnippetReadingException(
                $"""
                 No Key could be derived.
                 Path: {path}
                 Line: '{line}'
                 """);
        }

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