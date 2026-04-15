static class ExcludeToFilterBuilder
{
    public static ShouldIncludeDirectory ExcludesToFilter(List<string>? excludes) =>
        path =>
        {
            if (DefaultDirectoryExclusions.ShouldExcludeDirectory(path))
            {
                return false;
            }

            if(excludes == null ||
               excludes.Count == 0)
            {
                return true;
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

    public static ShouldIncludeFile? FileExcludesToFilter(List<string>? excludes)
    {
        if (excludes == null ||
            excludes.Count == 0)
        {
            return null;
        }

        var regexes = excludes
            .Select(GlobToRegex)
            .ToArray();

        return path =>
        {
            var name = Path.GetFileName(path);
            foreach (var regex in regexes)
            {
                if (regex.IsMatch(name))
                {
                    return false;
                }
            }

            return true;
        };
    }

    static Regex GlobToRegex(string glob)
    {
        var builder = new StringBuilder("^");
        foreach (var c in glob)
        {
            switch (c)
            {
                case '*':
                    builder.Append(".*");
                    break;
                case '?':
                    builder.Append('.');
                    break;
                default:
                    builder.Append(Regex.Escape(c.ToString()));
                    break;
            }
        }

        builder.Append('$');
        return new(builder.ToString(), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }
}