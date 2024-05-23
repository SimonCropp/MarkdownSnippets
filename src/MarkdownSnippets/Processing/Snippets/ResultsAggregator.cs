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

    public void AddMissing(MissingSnippet snippet) =>
        missingSnippets.Add(snippet);

    public void AddUsed(Include include) =>
        usedIncludes.Add(include);

    public void AddMissing(MissingInclude include) =>
        missingIncludes.Add(include);

    public void AddValidationError(ValidationError error) =>
        validationErrors.Add(error);

    public void AddValidationError(IEnumerable<ValidationError> errors) =>
        validationErrors.AddRange(errors);
}