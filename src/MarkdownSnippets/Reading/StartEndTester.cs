static class StartEndTester
{
    internal static bool IsStartOrEnd(CharSpan line)
    {
        var trimmedLine = line.Trim();
        return IsBeginSnippet(trimmedLine) ||
               IsEndSnippet(trimmedLine) ||
               IsStartRegion(trimmedLine) ||
               IsEndRegion(trimmedLine);
    }

    internal static bool IsStart(
        CharSpan trimmedLine,
        CharSpan path,
        out CharSpan currentKey,
        [NotNullWhen(true)] out EndFunc? endFunc,
        out CharSpan expressiveCode,
        out CharSpan language)
    {
        if (IsBeginSnippet(trimmedLine, path, out currentKey, out expressiveCode, out language))
        {
            endFunc = IsEndSnippet;
            return true;
        }

        if (IsStartRegion(trimmedLine, out currentKey))
        {
            endFunc = IsEndRegion;
            // not supported for regions
            expressiveCode = null;
            language = null;
            return true;
        }

        expressiveCode = null;
        language = null;
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

        if (substring.Contains(' ') ||
            !KeyValidator.IsValidKey(substring))
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
        out CharSpan expressiveCode) =>
        IsBeginSnippet(line, path, out key, out expressiveCode, out _);

    internal static bool IsBeginSnippet(
        CharSpan line,
        CharSpan path,
        out CharSpan key,
        out CharSpan expressiveCode,
        out CharSpan language)
    {
        expressiveCode = null;
        language = null;
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

            var args = substring[(startArgs + 1)..^1].Trim();
            args = ExtractLanguage(args, key, path, line, out language);
            expressiveCode = args;
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

    static CharSpan ExtractLanguage(CharSpan args, scoped CharSpan key, scoped CharSpan path, scoped CharSpan line, out CharSpan language)
    {
        language = null;
        if (!args.StartsWith("lang=", StringComparison.Ordinal))
        {
            return args;
        }

        var rest = args[5..];
        var end = rest.IndexOf(' ');
        CharSpan value;
        CharSpan remainder;
        if (end == -1)
        {
            value = rest;
            remainder = null;
        }
        else
        {
            value = rest[..end];
            remainder = rest[(end + 1)..].Trim();
        }

        if (value.Length == 0)
        {
            throw new SnippetReadingException(
                $"""
                 lang= must have a value.
                 Key: {key}
                 Path: {path}
                 Line: {line}
                 """);
        }

        foreach (var c in value)
        {
            if (c is (< 'a' or > 'z') and (< '0' or > '9'))
            {
                throw new SnippetReadingException(
                    $"""
                     lang value must be lowercase alphanumeric.
                     Key: {key}
                     Value: {value.ToString()}
                     Path: {path}
                     Line: {line}
                     """);
            }
        }

        language = value;
        return remainder;
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