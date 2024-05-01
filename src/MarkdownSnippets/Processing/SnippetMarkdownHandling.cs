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
        var builder = new StringBuilder();
        uint index = 0;
        foreach (var snippet in snippets)
        {
            builder.AppendLine(WriteSnippet(snippet, index));
            index++;
        }
        appendLine(builder.ToString().TrimEnd());
    }

    string WriteSnippet(Snippet snippet, uint index)
    {
        var builder = new StringBuilder();
        if (omitSnippetLinks)
        {
            builder.Append(WriteSnippetValueAndLanguage(snippet));
        }
        else
        {
            var anchor = GetAnchorText(snippet, index);
            var supText = GetSupText(snippet, anchor);

            builder.AppendLine($"<a id='{anchor}'></a>");
            builder.AppendLine(WriteSnippetValueAndLanguage(snippet));

            builder.Append($"<sup>{supText}</sup>");
        }

        return builder.ToString();
    }

    static string GetAnchorText(Snippet snippet, uint index)
    {
        var id = $"snippet-{snippet.Key}";
        if (index == 0)
        {
            return id;
        }

        return $"{id}-{index}";
    }

    string GetSupText(Snippet snippet, string anchor)
    {
        var linkForAnchor = $"<a href='#{anchor}' title='Start of snippet'>anchor</a>";
        if (snippet.Path == null || linkFormat == LinkFormat.None)
        {
            return linkForAnchor;
        }

        var path = snippet.Path.Replace('\\', '/');
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

    static string WriteSnippetValueAndLanguage(Snippet snippet) =>
        $"```{snippet.Language}\n{snippet.Value}\n```";

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