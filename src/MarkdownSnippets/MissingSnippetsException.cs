namespace MarkdownSnippets;

public class MissingSnippetsException :
    SnippetException
{
    public IReadOnlyList<MissingSnippet> Missing { get; }

    public MissingSnippetsException(IReadOnlyList<MissingSnippet> missing) :
        base($"Missing snippets:{Environment.NewLine}  {Report(missing)}") =>
        Missing = missing;

    static string Report(IReadOnlyList<MissingSnippet> missing) =>
        string.Join($"{Environment.NewLine}  ", missing.GroupBy(m => m.File ?? "file-unknown").Select(g => $"{g.Key}: {string.Join(",", g.Select(s => s.Key))}"));

    public override string ToString() => Message;
}
