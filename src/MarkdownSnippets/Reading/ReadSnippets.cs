namespace MarkdownSnippets;

[DebuggerDisplay("Count={Snippets.Count}")]
public class ReadSnippets :
    IEnumerable<Snippet>
{
    public IReadOnlyList<Snippet> Snippets { get; }
    public IReadOnlyList<string> Files { get; }
    public IReadOnlyDictionary<string, IReadOnlyList<Snippet>> Lookup { get; }
    public IReadOnlyList<Snippet> SnippetsInError { get; }

    public ReadSnippets(IReadOnlyList<Snippet> snippets, IReadOnlyList<string> files)
    {
        Snippets = snippets;
        Files = files;
        SnippetsInError = BuildErrors(snippets);
        Lookup = Snippets.ToDictionary();
    }

    // Single-pass replacement for Snippets.Where(IsInError).Distinct().ToList(). The common case
    // is zero errors, in which case we return Array.Empty rather than allocating a List + HashSet.
    static IReadOnlyList<Snippet> BuildErrors(IReadOnlyList<Snippet> snippets)
    {
        HashSet<Snippet>? seen = null;
        List<Snippet>? errors = null;
        foreach (var snippet in snippets)
        {
            if (!snippet.IsInError)
            {
                continue;
            }

            seen ??= [];
            if (seen.Add(snippet))
            {
                errors ??= [];
                errors.Add(snippet);
            }
        }

        return errors ?? (IReadOnlyList<Snippet>) Array.Empty<Snippet>();
    }

    /// <summary>
    /// Enumerates through the <see cref="Snippets" /> but will first throw an exception if there are any <see cref="SnippetsInError" />.
    /// </summary>
    public virtual IEnumerator<Snippet> GetEnumerator()
    {
        if (SnippetsInError.Count != 0)
        {
            throw new SnippetReadingException($"SnippetsInError: {string.Join(", ", SnippetsInError.Select(_ => _.Key))}");
        }

        return Snippets.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}