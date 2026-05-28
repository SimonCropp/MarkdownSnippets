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

        // Plain foreach over a span instead of LINQ Count(predicate). Count(predicate) on a
        // string boxes the IEnumerator<char> and allocates a delegate per call - real cost
        // because OrderBy(ScrubPath) calls this O(n log n) times per dictionary group.
        var count = 0;
        foreach (var ch in path.AsSpan())
        {
            if (ch is '/' or '\\')
            {
                count++;
            }
        }

        if (count == 0)
        {
            return path;
        }

        return string.Create(path.Length - count, path, (span, source) =>
        {
            var index = 0;
            foreach (var ch in source)
            {
                if (ch is not ('/' or '\\'))
                {
                    span[index++] = ch;
                }
            }
        });
    }
}
