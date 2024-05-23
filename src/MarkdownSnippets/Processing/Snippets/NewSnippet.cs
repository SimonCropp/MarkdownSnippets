using System.Diagnostics.CodeAnalysis;

class NewSnippet:ISnippetPart
{
    public bool ShouldExcludeFromIncludeProcessing(string lineCurrent) =>
        lineCurrent.StartsWith("snippet:", StringComparison.OrdinalIgnoreCase);


    public string Handle(MarkdownProcessor markdownProcessor, List<Line> lines, string? relativePath, int index, List<MissingSnippet> missingSnippets, List<Snippet> usedSnippets, string key, Line line, ResultsAggregator aggregator) =>
        markdownProcessor.ProcessSnippetLine(missingSnippets, usedSnippets, key, relativePath, line);

    public bool ExtractSnippet(Line line, [NotNullWhen(true)] out string? key)
    {
        var lineCurrent = line.Current;
        if (!ShouldExcludeFromIncludeProcessing(lineCurrent))
        {
            key = null;
            return false;
        }

        key = line.Current[8..]
            .Trim();
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new SnippetException($"Could not parse snippet from: {line.Original}. Path: {line.Path}. Line: {line.LineNumber}");
        }
        return true;
    }
}

public class ResultsAggregator
{
   internal List<Snippet> usedSnippets = [];
   internal List<MissingSnippet> missingSnippets = [];
   internal List<Include> usedIncludes = [];
   internal List<MissingInclude> missingIncludes = [];
   internal List<ValidationError> validationErrors = [];

    public void AddUsed(Snippet snippet) =>
        usedSnippets.Add(snippet);

    public void AddMissing(MissingSnippet missingSnippet) =>
        missingSnippets.Add(missingSnippet);

    public void AddUsed(Include include) =>
        usedIncludes.Add(include);

    public void AddMissing(MissingInclude missingInclude) =>
        missingIncludes.Add(missingInclude);

    public void AddValidationError(ValidationError validationError) =>
        validationErrors.Add(validationError);
}