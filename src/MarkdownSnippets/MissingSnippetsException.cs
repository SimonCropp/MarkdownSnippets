namespace MarkdownSnippets
{
    public class MissingSnippetsException :
        SnippetException
    {
        public IReadOnlyList<MissingSnippet> Missing { get; }

        public MissingSnippetsException(IReadOnlyList<MissingSnippet> missing) :
            base($"Missing snippets: {string.Join(", ", missing.Select(x => x.Key))}")
        {
            Missing = missing;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}