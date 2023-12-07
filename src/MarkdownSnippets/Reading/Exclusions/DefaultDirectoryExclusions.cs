namespace MarkdownSnippets;

public static class DefaultDirectoryExclusions
{
    public static bool ShouldExcludeDirectory(string path)
    {
        var suffix = Path.GetFileName(path).ToLowerInvariant();
        return suffix is
            ".git" or
            "packages" or
            "node_modules" or
            "bin" or
            "obj";
    }
}