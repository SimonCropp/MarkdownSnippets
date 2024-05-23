public class ResultsAggregator
{
    internal List<Snippet> usedSnippets = [];
    internal List<MissingSnippet> missingSnippets = [];
    internal List<Include> usedIncludes = [];
    internal List<MissingInclude> missingIncludes = [];
    internal List<ValidationError> validationErrors = [];

    public void AddUsed(Snippet snippet) =>
        usedSnippets.Add(snippet);
    public void AddUsed(IEnumerable<Snippet> snippets) =>
        usedSnippets.AddRange(snippets);

    public void AddMissing(MissingSnippet missingSnippet) =>
        missingSnippets.Add(missingSnippet);

    public void AddUsed(Include include) =>
        usedIncludes.Add(include);

    public void AddMissing(MissingInclude missingInclude) =>
        missingIncludes.Add(missingInclude);

    public void AddValidationError(ValidationError validationError) =>
        validationErrors.Add(validationError);

    public void AddValidationError(IEnumerable<ValidationError> validationError) =>
        validationErrors.AddRange(validationError);
}