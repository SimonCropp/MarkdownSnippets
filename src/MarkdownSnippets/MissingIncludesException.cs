namespace MarkdownSnippets;

public class MissingIncludesException(IReadOnlyList<MissingInclude> missing) :
    SnippetException($"Missing includes: {string.Join(", ", missing.Select(_ => _.Key))}")
{
    public IReadOnlyList<MissingInclude> Missing { get; } = missing;

    public override string ToString() => Message;
}