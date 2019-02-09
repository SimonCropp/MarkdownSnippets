using System;
using System.Collections.Generic;
using System.Linq;
using MarkdownSnippets;

static class SnippetExtensions
{
    public static Dictionary<string, IReadOnlyList<Snippet>> ToDictionary(this IEnumerable<Snippet> value)
    {
        return value
            .GroupBy(_ => _.Key.ToLowerInvariant(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                keySelector: _ => _.Key.ToLowerInvariant(),
                elementSelector: _ => _.ToReadonlyList(),
                comparer: StringComparer.OrdinalIgnoreCase);
    }
}