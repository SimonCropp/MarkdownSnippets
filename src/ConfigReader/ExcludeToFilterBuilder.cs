using MarkdownSnippets;

static class ExcludeToFilterBuilder
{
    public static ShouldIncludeDirectory ExcludesToFilter(List<string> excludes)
    {
        excludes = GetExcludesWithBothSlashes(excludes).ToList();
        return path =>
        {
            if (DefaultDirectoryExclusions.ShouldExcludeDirectory(path))
            {
                return false;
            }

            foreach (var exclude in excludes)
            {
                if (path.Contains(exclude))
                {
                    return false;
                }
            }
            return true;
        };
    }

    static IEnumerable<string> GetExcludesWithBothSlashes(List<string> excludes)
    {
        foreach (var exclude in excludes)
        {
            yield return exclude.Replace('\\', '/');
            yield return exclude.Replace('/', '\\');
        }
    }
}