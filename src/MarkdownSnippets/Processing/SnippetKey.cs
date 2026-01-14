static class SnippetKey
{
    public static bool ExtractStartCommentSnippet(Line line, [NotNullWhen(true)] out string? key)
    {
        var lineCurrent = line.Current.AsSpan().TrimStart();
        if (!IsStartCommentSnippetLine(lineCurrent))
        {
            key = null;
            return false;
        }

        var substring = lineCurrent[14..];
        var indexOf = substring.IndexOf("-->", StringComparison.Ordinal);
        if (indexOf < 0)
        {
            throw new SnippetException($"Could not find closing '-->' in: {line.Original}. Path: {line.Path}. Line: {line.LineNumber}");
        }

        key = substring[..indexOf].Trim().ToString();
        return true;
    }

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
    {
        var lineCurrent = line.Current.AsSpan().TrimStart();
        if (!IsSnippetLine(lineCurrent))
        {
            key = null;
            return false;
        }

        var keySpan = lineCurrent[8..].Trim();
        if (keySpan.IsWhiteSpace())
        {
            throw new SnippetException($"Could not parse snippet from: {line.Original}. Path: {line.Path}. Line: {line.LineNumber}");
        }

        key = keySpan.ToString();
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
