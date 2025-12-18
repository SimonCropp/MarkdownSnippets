namespace MarkdownSnippets;

/// <summary>
/// Handling to be passed to <see cref="MarkdownProcessor"/>.
/// </summary>
public class SnippetMarkdownHandling
{
    LinkFormat linkFormat;
    bool omitSnippetLinks;
    string? urlPrefix;
    string targetDirectory;

    public SnippetMarkdownHandling(string targetDirectory, LinkFormat linkFormat, bool omitSnippetLinks, string? urlPrefix = null)
    {
        this.linkFormat = linkFormat;
        this.omitSnippetLinks = omitSnippetLinks;
        this.urlPrefix = urlPrefix;
        Guard.AgainstNullAndEmpty(targetDirectory, nameof(targetDirectory));
        targetDirectory = Path.GetFullPath(targetDirectory);
        this.targetDirectory = targetDirectory.Replace('\\', '/');
    }

    public void Append(string key, IEnumerable<Snippet> snippets, Action<string> appendLine)
    {
        Guard.AgainstNullAndEmpty(key, nameof(key));
        uint index = 0;
        foreach (var snippet in snippets)
        {
            WriteSnippet(appendLine, snippet, index);
            index++;
        }
    }

    void WriteSnippet(Action<string> appendLine, Snippet snippet, uint index)
    {
        if (omitSnippetLinks)
        {
            WriteSnippetValueAndLanguage(appendLine, snippet);
            return;
        }

        var anchor = GetAnchorText(snippet, index);

        appendLine($"<a id='{anchor}'></a>");
        WriteSnippetValueAndLanguage(appendLine, snippet);

        var supText = GetSupText(snippet, anchor);
        appendLine($"<sup>{supText}</sup>");
    }

    static string GetAnchorText(Snippet snippet, uint index)
    {
        string id;
        var pathStr = snippet.Path;
        if (pathStr != null && pathStr.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            // For web-snippets, include the full URL and the snippet key, with '#' encoded
            var combined = $"snippet-{pathStr}#{snippet.Key}";
            id = combined.Replace("#", "%23");
        }
        else
        {
            id = $"snippet-{snippet.Key}";
        }

        if (index == 0)
        {
            return id;
        }

        return $"{id}-{index}";
    }

    string GetSupText(Snippet snippet, string anchor)
    {
        var linkForAnchor = $"<a href='#{anchor}' title='Start of snippet'>anchor</a>";
        var pathLocal = snippet.Path;
        if (pathLocal == null || linkFormat == LinkFormat.None)
        {
            return linkForAnchor;
        }

        var path = pathLocal.Replace('\\', '/');
        if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            // For web-snippets: use ViewUrl if provided, otherwise link back to the original URL with the snippet key as a hash
            if (snippet.ViewUrl != null)
            {
                return $"<a href='{snippet.ViewUrl}#L{snippet.StartLine}-L{snippet.EndLine}' title='Snippet source file'>anchor</a>";
            }
            return $"<a href='{path}#{snippet.Key}' title='Snippet source file'>anchor</a>";
        }
        if (!path.StartsWith(targetDirectory))
        {
            // if file is not in the targetDirectory then the url wont work
            return linkForAnchor;
        }

        path = path[targetDirectory.Length..];

        var builder = new StringBuilder($"<a href='{urlPrefix}");
        BuildLink(snippet, path, builder);
        Polyfill.Append(
            builder,
            $"' title='Snippet source file'>snippet source</a> | {linkForAnchor}");
        return builder.ToString();
    }

    static void WriteSnippetValueAndLanguage(Action<string> appendLine, Snippet snippet)
    {
        if (snippet.ExpressiveCode is null)
        {
            appendLine($"```{snippet.Language}");
        }
        else
        {
            appendLine($"```{snippet.Language} {snippet.ExpressiveCode}");
        }
        appendLine(snippet.Value);
        appendLine("```");
    }

    void BuildLink(Snippet snippet, string path, StringBuilder builder)
    {
        #region BuildLink
        switch (linkFormat)
        {
            case LinkFormat.GitHub:
                Polyfill.Append(builder, $"{path}#L{snippet.StartLine}-L{snippet.EndLine}");
                return;
            case LinkFormat.Tfs:
                Polyfill.Append(builder, $"{path}&line={snippet.StartLine}&lineEnd={snippet.EndLine}");
                return;
            case LinkFormat.Bitbucket:
                Polyfill.Append(builder, $"{path}#lines={snippet.StartLine}:{snippet.EndLine}");
                return;
            case LinkFormat.GitLab:
                Polyfill.Append(builder, $"{path}#L{snippet.StartLine}-{snippet.EndLine}");
                return;
            case LinkFormat.DevOps:
                Polyfill.Append(builder, $"?path={path}&line={snippet.StartLine}&lineEnd={snippet.EndLine}&lineStartColumn=1&lineEndColumn=999");
                return;
            case LinkFormat.None:
                throw new($"Unknown LinkFormat: {linkFormat}");
        }
        #endregion
    }
}