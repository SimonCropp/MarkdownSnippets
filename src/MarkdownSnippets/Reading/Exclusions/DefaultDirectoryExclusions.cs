namespace MarkdownSnippets;

public static class DefaultDirectoryExclusions
{
    public static bool ShouldExcludeDirectory(string path)
    {
        var suffix = Path
            .GetFileName(path)
            .ToLowerInvariant();
        if (suffix is
            ".git" or
            ".vs" or
            ".idea" or
            "packages" or
            "node_modules" or
            "bin" or
            "obj")
        {
            return true;
        }

        var directory = new DirectoryInfo(path);
        return directory.Attributes.HasFlag(FileAttributes.Hidden);
    }
}