static class SnippetKey
{
    public static bool ExtractStartCommentSnippet(Line line, [NotNullWhen(true)] out string? key)
    {
        var lineCurrent = line.Current;
        if (!IsStartCommentSnippetLine(lineCurrent))
        {
            key = null;
            return false;
        }

        var substring = line.Current[14..];
        var indexOf = substring.IndexOf("-->");
        key = substring[..indexOf]
            .Trim();
        return true;
    }

    public static bool ExtractSnippet(Line line, [NotNullWhen(true)] out string? key)
    {
        var lineCurrent = line.Current;
        if (!IsSnippetLine(lineCurrent))
        {
            key = null;
            return false;
        }

        key = line.Current[8..]
            .Trim();
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new SnippetException($"Could not parse snippet from: {line.Original}. Path: {line.Path}. Line: {line.LineNumber}");
        }

        return true;
    }

    public static bool ExtractWebSnippet(Line line, [NotNullWhen(true)] out string? url, [NotNullWhen(true)] out string? snippetKey)
    {
        var lineCurrent = line.Current;
        if (!IsWebSnippetLine(lineCurrent))
        {
            url = null;
            snippetKey = null;
            return false;
        }
        var value = lineCurrent[12..].Trim(); // after 'web-snippet:', fixed from 11 to 12
        var hashIndex = value.LastIndexOf('#');
        if (hashIndex < 0 || hashIndex == value.Length - 1)
        {
            url = null;
            snippetKey = null;
            return false;
        }
        url = value[..hashIndex];
        snippetKey = value[(hashIndex + 1)..];
        return true;
    }

    public static bool IsSnippetLine(string line) =>
        line.StartsWith("snippet:", StringComparison.OrdinalIgnoreCase);

    public static bool IsStartCommentSnippetLine(string line) =>
        line.StartsWith("<!-- snippet:", StringComparison.OrdinalIgnoreCase);

    public static bool IsWebSnippetLine(string line) =>
        line.StartsWith("web-snippet:", StringComparison.OrdinalIgnoreCase);
}