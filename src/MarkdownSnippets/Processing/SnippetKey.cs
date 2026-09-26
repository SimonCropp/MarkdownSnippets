static class SnippetKey
{
    public static bool ExtractStartCommentSnippet(Line line, [NotNullWhen(true)] out string? key)
        => ExtractStartCommentSnippet(line, out key, out _, out _);

    public static bool ExtractStartCommentSnippet(
        Line line,
        [NotNullWhen(true)] out string? key,
        out string? language,
        out string? expressiveCode)
    {
        var lineCurrent = line.Current.AsSpan().TrimStart();
        if (!IsStartCommentSnippetLine(lineCurrent))
        {
            key = null;
            language = null;
            expressiveCode = null;
            return false;
        }

        var substring = lineCurrent[14..];
        var indexOf = substring.IndexOf("-->", StringComparison.Ordinal);
        if (indexOf < 0)
        {
            throw new SnippetException($"Could not find closing '-->' in: {line.Original}. Path: {line.Path}. Line: {line.LineNumber}");
        }

        ExtractUrlMetadata(line, substring[..indexOf].Trim(), out var keyWithoutMetadata, out language, out expressiveCode);
        key = keyWithoutMetadata.ToString();
        return true;
    }

    static void ExtractUrlMetadata(
        Line line,
        CharSpan value,
        out CharSpan key,
        out string? language,
        out string? expressiveCode)
    {
        key = value;
        language = null;
        expressiveCode = null;

        if (value.Length < 3 || value[^1] != ')')
        {
            return;
        }

        var metadataStart = -1;
        for (var index = 1; index < value.Length - 1; index++)
        {
            if (value[index] == '(' && char.IsWhiteSpace(value[index - 1]) && IsHttpUrl(value[..index].TrimEnd()))
            {
                metadataStart = index;
                break;
            }
        }

        if (metadataStart == -1)
        {
            return;
        }

        var url = value[..metadataStart].TrimEnd();
        key = url;
        var metadata = value[(metadataStart + 1)..^1].Trim();
        var remainingMetadata = StartEndTester.ExtractLanguage(
            metadata,
            url,
            line.Path.AsSpan(),
            line.Original.AsSpan(),
            out var languageValue);
        language = languageValue.IsEmpty ? null : languageValue.ToString();
        expressiveCode = remainingMetadata.IsEmpty ? null : remainingMetadata.ToString();
    }

    static bool IsHttpUrl(CharSpan value) =>
        value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
        value.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

    public static bool ExtractStartCommentWebSnippet(Line line, [NotNullWhen(true)] out string? url, [NotNullWhen(true)] out string? snippetKey) =>
        ExtractStartCommentWebSnippet(line, out url, out snippetKey, out _);

    public static bool ExtractStartCommentWebSnippet(Line line, [NotNullWhen(true)] out string? url, [NotNullWhen(true)] out string? snippetKey, out string? viewUrl)
    {
        var lineCurrent = line.Current.AsSpan().TrimStart();
        if (!IsStartCommentWebSnippetLine(lineCurrent))
        {
            url = null;
            snippetKey = null;
            viewUrl = null;
            return false;
        }

        var substring = lineCurrent[18..]; // after "<!-- web-snippet: "
        var indexOf = substring.IndexOf("-->", StringComparison.Ordinal);
        if (indexOf < 0)
        {
            throw new SnippetException($"Could not find closing '-->' in: {line.Original}. Path: {line.Path}. Line: {line.LineNumber}");
        }

        var value = substring[..indexOf].Trim();

        // Check for optional second URL separated by whitespace
        var firstSpaceIndex = value.IndexOfAny([' ', '\t']);
        CharSpan firstPart;
        if (firstSpaceIndex >= 0)
        {
            firstPart = value[..firstSpaceIndex];
            var secondPart = value[(firstSpaceIndex + 1)..].TrimStart();
            var nextSpace = secondPart.IndexOfAny([' ', '\t']);
            viewUrl = (nextSpace >= 0 ? secondPart[..nextSpace] : secondPart).ToString();
        }
        else
        {
            firstPart = value;
            viewUrl = null;
        }

        var hashIndex = firstPart.LastIndexOf('#');
        if (hashIndex < 0 || hashIndex == firstPart.Length - 1)
        {
            url = null;
            snippetKey = null;
            viewUrl = null;
            return false;
        }
        url = firstPart[..hashIndex].ToString();
        snippetKey = firstPart[(hashIndex + 1)..].ToString();
        return true;
    }

    public static bool ExtractSnippet(Line line, [NotNullWhen(true)] out string? key)
        => ExtractSnippet(line, out key, out _, out _);

    public static bool ExtractSnippet(
        Line line,
        [NotNullWhen(true)] out string? key,
        out string? language,
        out string? expressiveCode)
    {
        var lineCurrent = line.Current.AsSpan().TrimStart();
        if (!IsSnippetLine(lineCurrent))
        {
            key = null;
            language = null;
            expressiveCode = null;
            return false;
        }

        var keySpan = lineCurrent[8..].Trim();
        if (keySpan.IsWhiteSpace())
        {
            throw new SnippetException($"Could not parse snippet from: {line.Original}. Path: {line.Path}. Line: {line.LineNumber}");
        }

        ExtractUrlMetadata(line, keySpan, out var keyWithoutMetadata, out language, out expressiveCode);
        key = keyWithoutMetadata.ToString();
        return true;
    }

    public static bool ExtractWebSnippet(Line line, [NotNullWhen(true)] out string? url, [NotNullWhen(true)] out string? snippetKey) =>
        ExtractWebSnippet(line, out url, out snippetKey, out _);

    public static bool ExtractWebSnippet(Line line, [NotNullWhen(true)] out string? url, [NotNullWhen(true)] out string? snippetKey, out string? viewUrl)
    {
        var lineCurrent = line.Current.AsSpan().TrimStart();
        if (!IsWebSnippetLine(lineCurrent))
        {
            url = null;
            snippetKey = null;
            viewUrl = null;
            return false;
        }
        var value = lineCurrent[12..].Trim(); // after 'web-snippet:'

        // Check for optional second URL separated by whitespace
        var firstSpaceIndex = value.IndexOfAny([' ', '\t']);
        CharSpan firstPart;
        if (firstSpaceIndex >= 0)
        {
            firstPart = value[..firstSpaceIndex];
            var secondPart = value[(firstSpaceIndex + 1)..].TrimStart();
            var nextSpace = secondPart.IndexOfAny([' ', '\t']);
            viewUrl = (nextSpace >= 0 ? secondPart[..nextSpace] : secondPart).ToString();
        }
        else
        {
            firstPart = value;
            viewUrl = null;
        }

        var hashIndex = firstPart.LastIndexOf('#');
        if (hashIndex < 0 || hashIndex == firstPart.Length - 1)
        {
            url = null;
            snippetKey = null;
            viewUrl = null;
            return false;
        }
        url = firstPart[..hashIndex].ToString();
        snippetKey = firstPart[(hashIndex + 1)..].ToString();
        return true;
    }

    public static bool IsSnippetLine(string line) =>
        IsSnippetLine(line.AsSpan());

    public static bool IsSnippetLine(CharSpan line) =>
        line.StartsWith("snippet:", StringComparison.OrdinalIgnoreCase);

    public static bool IsStartCommentSnippetLine(string line) =>
        IsStartCommentSnippetLine(line.AsSpan());

    public static bool IsStartCommentSnippetLine(CharSpan line) =>
        line.StartsWith("<!-- snippet:", StringComparison.OrdinalIgnoreCase);

    public static bool IsWebSnippetLine(string line) =>
        IsWebSnippetLine(line.AsSpan());

    public static bool IsWebSnippetLine(CharSpan line) =>
        line.StartsWith("web-snippet:", StringComparison.OrdinalIgnoreCase);

    public static bool IsStartCommentWebSnippetLine(string line) =>
        IsStartCommentWebSnippetLine(line.AsSpan());

    public static bool IsStartCommentWebSnippetLine(CharSpan line) =>
        line.StartsWith("<!-- web-snippet:", StringComparison.OrdinalIgnoreCase);
}
