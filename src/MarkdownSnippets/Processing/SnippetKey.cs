static class SnippetKey
{
    public static bool ExtractStartCommentSnippet(Line line, [NotNullWhen(true)] out string? key)
    {
        var lineCurrent = line.Current.TrimStart();
        if (!IsStartCommentSnippetLine(lineCurrent))
        {
            key = null;
            return false;
        }

        var substring = lineCurrent[14..];
        var indexOf = substring.IndexOf("-->");
        key = substring[..indexOf]
            .Trim();
        return true;
    }

    public static bool ExtractStartCommentWebSnippet(Line line, [NotNullWhen(true)] out string? url, [NotNullWhen(true)] out string? snippetKey) =>
        ExtractStartCommentWebSnippet(line, out url, out snippetKey, out _);

    public static bool ExtractStartCommentWebSnippet(Line line, [NotNullWhen(true)] out string? url, [NotNullWhen(true)] out string? snippetKey, out string? viewUrl)
    {
        var lineCurrent = line.Current.TrimStart();
        if (!IsStartCommentWebSnippetLine(lineCurrent))
        {
            url = null;
            snippetKey = null;
            viewUrl = null;
            return false;
        }

        var substring = lineCurrent[18..]; // after "<!-- web-snippet: "
        var indexOf = substring.IndexOf("-->");
        var value = substring[..indexOf].Trim();

        // Check for optional second URL separated by whitespace
        var parts = value.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
        string firstPart;
        if (parts.Length >= 2)
        {
            firstPart = parts[0];
            viewUrl = parts[1];
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
        url = firstPart[..hashIndex];
        snippetKey = firstPart[(hashIndex + 1)..];
        return true;
    }

    public static bool ExtractSnippet(Line line, [NotNullWhen(true)] out string? key)
    {
        var lineCurrent = line.Current.TrimStart();
        if (!IsSnippetLine(lineCurrent))
        {
            key = null;
            return false;
        }

        key = lineCurrent[8..]
            .Trim();
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new SnippetException($"Could not parse snippet from: {line.Original}. Path: {line.Path}. Line: {line.LineNumber}");
        }

        return true;
    }

    public static bool ExtractWebSnippet(Line line, [NotNullWhen(true)] out string? url, [NotNullWhen(true)] out string? snippetKey) =>
        ExtractWebSnippet(line, out url, out snippetKey, out _);

    public static bool ExtractWebSnippet(Line line, [NotNullWhen(true)] out string? url, [NotNullWhen(true)] out string? snippetKey, out string? viewUrl)
    {
        var lineCurrent = line.Current.TrimStart();
        if (!IsWebSnippetLine(lineCurrent))
        {
            url = null;
            snippetKey = null;
            viewUrl = null;
            return false;
        }
        var value = lineCurrent[12..].Trim(); // after 'web-snippet:'

        // Check for optional second URL separated by whitespace
        var parts = value.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
        string firstPart;
        if (parts.Length >= 2)
        {
            firstPart = parts[0];
            viewUrl = parts[1];
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
        url = firstPart[..hashIndex];
        snippetKey = firstPart[(hashIndex + 1)..];
        return true;
    }

    public static bool IsSnippetLine(string line) =>
        line.StartsWith("snippet:", StringComparison.OrdinalIgnoreCase);

    public static bool IsStartCommentSnippetLine(string line) =>
        line.StartsWith("<!-- snippet:", StringComparison.OrdinalIgnoreCase);

    public static bool IsWebSnippetLine(string line) =>
        line.StartsWith("web-snippet:", StringComparison.OrdinalIgnoreCase);

    public static bool IsStartCommentWebSnippetLine(string line) =>
        line.StartsWith("<!-- web-snippet:", StringComparison.OrdinalIgnoreCase);
}