using System.Diagnostics.CodeAnalysis;

class ReplaceSnippet : ISnippetPart
{
    public bool ShouldExcludeFromIncludeProcessing(string lineCurrent) =>
        lineCurrent.StartsWith("<!-- snippet:", StringComparison.OrdinalIgnoreCase);

    public string Handle(MarkdownProcessor markdownProcessor, List<Line> lines, string? relativePath, int index, List<MissingSnippet> missingSnippets, List<Snippet> usedSnippets, string key, Line line, ResultsAggregator aggregator)
    {
        lines.RemoveUntil(
            index + 1,
            "<!-- endSnippet -->",
            relativePath,
            line);
        return markdownProcessor.ProcessSnippetLine(missingSnippets, usedSnippets, key, relativePath, line, aggregator);
    }

    public bool ExtractSnippet(Line line, [NotNullWhen(true)] out string? key)
    {
        var lineCurrent = line.Current;
        if (!ShouldExcludeFromIncludeProcessing(lineCurrent))
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
}