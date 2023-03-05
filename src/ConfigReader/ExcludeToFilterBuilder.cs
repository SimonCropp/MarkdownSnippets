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
                var excludeSpan = exclude.AsSpan();

                if (path.Contains(excludeSpan))
                {
                    return true;
                }

                if (pathForwardSlash.Contains(excludeSpan))
                {
                    return true;
                }

                if (pathBackSlash.Contains(excludeSpan))
                {
                    return true;
                }
            }

            return false;
        };
}