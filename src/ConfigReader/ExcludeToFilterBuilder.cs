using MarkdownSnippets;

static class ExcludeToFilterBuilder
{
    public static ShouldIncludeDirectory ExcludesToFilter(List<string> excludes) =>
        path =>
        {
            var pathSpan = path.AsSpan();

            if (DefaultDirectoryExclusions.ShouldExcludeDirectory(path))
            {
                return false;
            }

            var pathForwardSlash = pathSpan.Replace('\\', '/');
            var pathBackSlash = pathSpan.Replace('/', '\\');
            foreach (var exclude in excludes)
            {
                var excludeSpan = exclude.AsSpan();

                if (pathSpan.Contains(excludeSpan))
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