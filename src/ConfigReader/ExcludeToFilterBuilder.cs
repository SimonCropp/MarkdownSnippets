static class ExcludeToFilterBuilder
{
    public static ShouldIncludeDirectory ExcludesToFilter(List<string> excludes) =>
        path =>
        {
            if (DefaultDirectoryExclusions.ShouldExcludeDirectory(path))
            {
                return false;
            }

            if (!excludes.Any(path.Contains))
            {
                return true;
            }

            if (!excludes.Any(path.Replace('\\', '/').Contains))
            {
                return true;
            }

            if (!excludes.Any(path.Replace('/', '\\').Contains))
            {
                return true;
            }

            return false;
        };
}