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

            var pathForwardSlash = path.Replace('\\', '/');
            var pathBackSlash = path.Replace('/', '\\');
            foreach (var exclude in excludes)
            {
                if (path.Contains(exclude))
                {
                    return true;
                }

                if (pathForwardSlash.Contains(exclude))
                {
                    return true;
                }

                if (pathBackSlash.Contains(exclude))
                {
                    return true;
                }
            }

            return false;
        };
}