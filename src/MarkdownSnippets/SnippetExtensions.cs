static class SnippetExtensions
{
    public static Dictionary<string, IReadOnlyList<Snippet>> ToDictionary(this IEnumerable<Snippet> value) =>
        value
            .GroupBy(_ => _.Key)
            .ToDictionary(
                keySelector: _ => _.Key,
                elementSelector: _ => _.OrderBy(ScrubPath).ToReadonlyList());

    static string? ScrubPath(Snippet snippet) =>
        snippet.Path?
            .Replace("/", "")
            .Replace("\\", "");
}