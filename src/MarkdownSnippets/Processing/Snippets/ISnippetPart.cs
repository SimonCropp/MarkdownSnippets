using System.Diagnostics.CodeAnalysis;

public interface ISnippetPart
{
    string Handle(MarkdownProcessor markdownProcessor, List<Line> lines, string? relativePath, int index, List<MissingSnippet> missingSnippets, List<Snippet> usedSnippets, string key, Line line, ResultsAggregator aggregator);
    bool ExtractSnippet(Line line, [NotNullWhen(true)] out string? key);
    public bool ShouldExcludeFromIncludeProcessing(string line);
}