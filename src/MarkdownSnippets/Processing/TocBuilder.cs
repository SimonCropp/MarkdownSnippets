using System.Collections.Generic;
using System.Text;

static class TocBuilder
{
    public static string BuildToc(List<Line> headerLines)
    {
        var builder = new StringBuilder(@"<!-- toc -->
## Contents

");
        foreach (var headerLine in headerLines)
        {
            var title = headerLine.Current.Substring(3).Trim();
            var link = title.ToLowerInvariant().Replace(' ', '-');
            builder.AppendLine($" * [{title}](#{link})");
        }

        return builder.ToString();
    }
}