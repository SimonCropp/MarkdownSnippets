namespace MarkdownSnippets;

public class MissingSnippetsException(IReadOnlyList<MissingSnippet> missing) :
    SnippetException($"Missing snippets:{Environment.NewLine}  {Report(missing)}")
{
    public IReadOnlyList<MissingSnippet> Missing { get; } = missing;

    static string Report(IReadOnlyList<MissingSnippet> missing) =>
        string.Join($"{Environment.NewLine}  ", missing.GroupBy(m => m.File ?? "file-unknown").Select(g => $"{g.Key}: {string.Join(",", g.Select(s => s.Key))}"));

    public override string ToString() => Message;
}
