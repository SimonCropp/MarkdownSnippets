static class SnippetExtensions
{
    public static Dictionary<string, IReadOnlyList<Snippet>> ToDictionary(this IEnumerable<Snippet> value) =>
        value
            .GroupBy(_ => _.Key)
            .ToDictionary(
                keySelector: _ => _.Key,
                elementSelector: _ => _.OrderBy(ScrubPath).ToReadonlyList());

    static string? ScrubPath(Snippet snippet)
    {
        var path = snippet.Path;
        if (path == null)
        {
            return null;
        }

        var builder = StringBuilderCache.Acquire(path.Length);
        foreach (var ch in path)
        {
            if (ch is not ('/' or '\\'))
            {
                builder.Append(ch);
            }
        }

        return StringBuilderCache.GetStringAndRelease(builder);
    }
}