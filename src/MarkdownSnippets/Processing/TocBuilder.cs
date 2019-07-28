using System.Collections.Generic;
using System.Linq;
using System.Text;

static class TocBuilder
{
    public static string BuildToc(List<Line> headerLines)
    {
        var processed = new List<string>();
        var builder = new StringBuilder(@"<!-- toc -->
## Contents

");
        foreach (var headerLine in headerLines)
        {
            var title = headerLine.Current.Substring(3).Trim();
            var link = BuildLink(processed, title);
            builder.AppendLine($" * [{title}](#{link})");
            processed.Add(title);
        }

        return builder.ToString();
    }

    static string BuildLink(List<string> processed, string title)
    {
        var processedCount = processed.Count(x => x == title);
        var lowerTitle = title.ToLowerInvariant();
        var noSpaces = lowerTitle.Replace(' ', '-');
        if (processedCount == 0)
        {
            return noSpaces;
        }

        return $"{noSpaces}-{processedCount}";
    }
}