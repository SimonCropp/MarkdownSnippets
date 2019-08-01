using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

static class TocBuilder
{
    public static string BuildToc(List<Line> headerLines, int level, List<string> tocExcludes)
    {
        var processed = new List<string>();
        var builder = new StringBuilder(@"<!-- toc -->
## Contents

");
        const int startingLevel = 2;
        var headerDepth = level + startingLevel - 1;
        var headingCount = 0;
        foreach (var headerLine in headerLines)
        {
            var current = headerLine.Current;
            var trimmedHash = current.TrimStart('#');
            if (!trimmedHash.StartsWith(" "))
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

            var title = GetTitle(current);
            if (tocExcludes.Contains(title))
            {
                continue;
            }

            headingCount++;

            var link = BuildLink(processed, title);
            var indent = new string(' ', (headerLevel - 1) * 2);
            builder.AppendLine($"{indent}* [{title}](#{link})");
        }

        if (headingCount == 0)
        {
            return string.Empty;
        }

        builder.AppendLine("<!-- endtoc -->");
        return builder.ToString();
    }

    static string GetTitle(string current)
    {
        var trim = current.Substring(3).Trim();
        return Markdown.StripMarkdown(trim);
    }

    static string BuildLink(List<string> processed, string title)
    {
        var lowerTitle = title.ToLowerInvariant();
        var processedCount = processed.Count(x => x == lowerTitle);
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
        lowerTitle = new string(lowerTitle.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-' || c == '_').ToArray());
        return lowerTitle.Replace(' ', '-');
    }
}