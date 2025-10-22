static class StartEndTester
{
    internal static bool IsStartOrEnd(CharSpan trimmedLine) =>
        IsBeginSnippet(trimmedLine) ||
        IsEndSnippet(trimmedLine) ||
        IsStartRegion(trimmedLine) ||
        IsEndRegion(trimmedLine);

    internal static bool IsStart(
        CharSpan trimmedLine,
        CharSpan path,
        out CharSpan currentKey,
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

    static bool IsEndRegion(CharSpan line) =>
        line.StartsWith("#endregion", StringComparison.Ordinal);

    static bool IsEndSnippet(CharSpan line) =>
        IndexOf(line, "end-snippet") >= 0;

    static bool IsStartRegion(CharSpan line) =>
        line.StartsWith("#region ", StringComparison.Ordinal);

    internal static bool IsStartRegion(
        CharSpan line,
        out CharSpan key)
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

        if (!KeyValidator.IsValidKey(substring))
        {
            key = null;
            return false;
        }

        key = substring.ToString();
        return true;
    }

    static bool IsBeginSnippet(CharSpan line)
    {
        var startIndex = IndexOf(line, "begin-snippet: ");
        return startIndex != -1;
    }

    internal static bool IsBeginSnippet(
        CharSpan line,
        CharSpan path,
        out CharSpan key,
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

            var expressiveCodeSpan = substring[(startArgs + 1)..^1].Trim();
            if (expressiveCodeSpan.Length != 0)
            {
                expressiveCode = expressiveCodeSpan.ToString();
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

        if (KeyValidator.IsValidKey(key))
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

    static int IndexOf(CharSpan line, CharSpan value)
    {
        if (value.Length > line.Length)
        {
            return -1;
        }

        var charactersToScan = Math.Min(line.Length, value.Length + 10);
        return line[..charactersToScan].IndexOf(value, StringComparison.Ordinal);
    }
}