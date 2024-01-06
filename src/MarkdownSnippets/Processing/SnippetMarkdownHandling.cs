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

        var builder = new StringBuilder("<a href='");
        builder.Append(urlPrefix);
        BuildLink(snippet, path, builder);
        builder.Append("' title='Snippet source file'>snippet source</a> | ");
        builder.Append(linkForAnchor);
        return builder.ToString();
    }

    static void WriteSnippetValueAndLanguage(Action<string> appendLine, Snippet snippet)
    {
        appendLine($"```{snippet.Language}");
        appendLine(snippet.Value);
        appendLine("```");
    }

    void BuildLink(Snippet snippet, string path, StringBuilder builder)
    {
        #region BuildLink
        if (linkFormat == LinkFormat.GitHub)
        {
            builder.Append(path);
            builder.Append("#L");
            builder.Append(snippet.StartLine);
            builder.Append("-L");
            builder.Append(snippet.EndLine);
            return;
        }

        if (linkFormat == LinkFormat.Tfs)
        {
            builder.Append(path);
            builder.Append("&line=");
            builder.Append(snippet.StartLine);
            builder.Append("&lineEnd=");
            builder.Append(snippet.EndLine);
            return;
        }

        if (linkFormat == LinkFormat.Bitbucket)
        {
            builder.Append(path);
            builder.Append("#lines");
            builder.Append(snippet.StartLine);
            builder.Append(":");
            builder.Append(snippet.EndLine);
            return;
        }

        if (linkFormat == LinkFormat.GitLab)
        {
            builder.Append(path);
            builder.Append("#L");
            builder.Append(snippet.StartLine);
            builder.Append("-");
            builder.Append(snippet.EndLine);
            return;
        }
        #endregion

        throw new($"Unknown LinkFormat: {linkFormat}");
    }
}