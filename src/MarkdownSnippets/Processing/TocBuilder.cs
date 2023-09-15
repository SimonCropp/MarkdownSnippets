static class TocBuilder
{
    public static string BuildToc(List<Line> headerLines, int level, List<string> tocExcludes, string newLine)
    {
        var processed = new List<string>();
        var builder = new StringBuilder();
        builder.Append("<!-- toc -->");
        builder.Append(newLine);
        builder.Append("## Contents");
        builder.Append(newLine);
        builder.Append(newLine);
        const int startingLevel = 2;
        var headerDepth = level + startingLevel - 1;
        var headingCount = 0;
        foreach (var headerLine in headerLines)
        {
            var current = headerLine.Current;
            var trimmedHash = current.TrimStart('#');
            if (!trimmedHash.StartsWith(' '))
            {
                continue;
            }

            var headerLevel = current.Length - trimmedHash.Length;
            if (headerLevel == 1)
            {
                continue;
            }

            if (headerLevel > headerDepth)
            {
                continue;
            }

            var title = GetTitle(trimmedHash);
            if (tocExcludes.Any(_ => string.Equals(_, title, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            headingCount++;

            var link = BuildLink(processed, title);
            var indent = new string(' ', (headerLevel - 1) * 2);
            builder.Append($"{indent}* [{title}](#{link})");
            builder.Append(newLine);
        }

        if (headingCount == 0)
        {
            return $"<!-- toc -->{newLine}<!-- endToc -->";
        }

        builder.TrimEnd();
        builder.Append("<!-- endToc -->");

        return builder.ToString();
    }

    static string GetTitle(string current)
    {
        var trim = current[1..].Trim();
        return Markdown.StripMarkdown(trim);
    }

    static string BuildLink(List<string> processed, string title)
    {
        var lowerTitle = title.ToLowerInvariant();
        var processedCount = processed.Count(_ => _ == lowerTitle);
        processed.Add(lowerTitle);
        var noSpaces = SanitizeLink(lowerTitle);
        if (processedCount == 0)
        {
            return noSpaces;
        }

        return $"{noSpaces}-{processedCount}";
    }

    internal static string SanitizeLink(string lowerTitle)
    {
        lowerTitle = new(lowerTitle
            .Where(_ => char.IsLetterOrDigit(_) ||
                        char.IsWhiteSpace(_) ||
                        _ is
                            '-' or
                            '_')
            .ToArray());
        return lowerTitle.Replace(' ', '-');
    }
}