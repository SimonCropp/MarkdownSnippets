using MarkdownSnippets;

static class ExcludeToFilterBuilder
{
    public static ShouldIncludeDirectory ExcludesToFilter(List<string> excludes) =>
        path =>
        {
            if (DefaultDirectoryExclusions.ShouldExcludeDirectory(path))
            {
                return false;
            }

            excludes = GetExcludesWithBothSlashes(excludes).ToList();

            return !excludes.Any(path.Contains);
        };

    static IEnumerable<string> GetExcludesWithBothSlashes(List<string> excludes)
    {
        foreach (var exclude in excludes)
        {
            yield return exclude.Replace('\\', '/');
            yield return exclude.Replace('/', '\\');
        }
    }
}