namespace MarkdownSnippets;

public class MissingSnippetsException(IReadOnlyList<MissingSnippet> missing) :
    SnippetException($"Missing snippets:{Environment.NewLine}  {Report(missing)}")
{
    public IReadOnlyList<MissingSnippet> Missing { get; } = missing;

    static string Report(IReadOnlyList<MissingSnippet> missing) =>
        string.Join(
            $"{Environment.NewLine}  ",
            missing.GroupBy(_ => _.File ?? "file-unknown")
                .Select(_ => $"{_.Key}: {string.Join(",", _.Select(s => s.Key))}"));

    public override string ToString() => Message;
}
