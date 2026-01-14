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

        var count = path.Count(_ => _ is '/' or '\\');
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
