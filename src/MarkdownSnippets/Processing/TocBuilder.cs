static class TocBuilder
{
    public static string BuildToc(List<Line> headerLines, int level, List<string> tocExcludes, string newLine)
    {
        var excludesSet = new HashSet<string>(tocExcludes, StringComparer.OrdinalIgnoreCase);
        var processed = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
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
            if (excludesSet.Contains(title))
            {
                continue;
            }

            headingCount++;

            builder.Append(' ', (headerLevel - 1) * 2);
            builder.Append("* [");
            builder.Append(title);
            builder.Append("](#");
            BuildLink(builder, processed, title);
            builder.Append(')');
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

    static void BuildLink(StringBuilder builder, Dictionary<string, int> processed, string title)
    {
        var lowerTitle = title.ToLowerInvariant();
        processed.TryGetValue(lowerTitle, out var processedCount);
        processed[lowerTitle] = processedCount + 1;
        SanitizeLink(builder, lowerTitle);
        if (processedCount > 0)
        {
            builder.Append('-');
            builder.Append(processedCount);
        }
    }

    internal static void SanitizeLink(StringBuilder builder, string lowerTitle)
    {
        foreach (var ch in lowerTitle)
        {
            if (char.IsLetterOrDigit(ch) || ch is '-' or '_')
            {
                builder.Append(ch);
            }
            else if (char.IsWhiteSpace(ch))
            {
                builder.Append('-');
            }
        }
    }
}