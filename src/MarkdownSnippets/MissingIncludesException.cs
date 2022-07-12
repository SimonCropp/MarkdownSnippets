namespace MarkdownSnippets;

public class MissingIncludesException :
    SnippetException
{
    public IReadOnlyList<MissingInclude> Missing { get; }

    public MissingIncludesException(IReadOnlyList<MissingInclude> missing) :
        base($"Missing includes: {string.Join(", ", missing.Select(_ => _.Key))}") =>
        Missing = missing;

    public override string ToString() => Message;
}