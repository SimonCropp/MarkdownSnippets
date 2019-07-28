using System.Collections.Generic;
using System.Linq;
using System.Text;

static class TocBuilder
{
    public static string BuildToc(List<Line> headerLines, int level)
    {
        var processed = new List<string>();
        var builder = new StringBuilder(@"<!-- toc -->
## Contents

");
        const int startingLevel = 2;
        var headerDepth = level + startingLevel;
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

            headingCount++;

            var title = current.Substring(3).Trim();
            var link = BuildLink(processed, title);
            var indent = new string(' ', headerLevel-1);
            builder.AppendLine($"{indent}* [{title}](#{link})");
        }

        if (headingCount == 0)
        {
            return string.Empty;
        }
        builder.AppendLine("<!-- endtoc -->");
        return builder.ToString();
    }

    static string BuildLink(List<string> processed, string title)
    {
        var lowerTitle = title.ToLowerInvariant();
        var processedCount = processed.Count(x => x == lowerTitle);
        processed.Add(lowerTitle);
        var noSpaces = lowerTitle.Replace(' ', '-');
        if (processedCount == 0)
        {
            return noSpaces;
        }

        return $"{noSpaces}-{processedCount}";
    }
}