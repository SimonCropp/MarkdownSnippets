namespace MarkdownSnippets;

public static class DefaultDirectoryExclusions
{
    public static bool ShouldExcludeDirectory(CharSpan path)
    {
        var suffix = PathEx.GetFileName(path);
        return suffix.StartsWith('.') ||
               suffix is
                   "packages" or
                   "node_modules" or
                   "bin" or
                   "obj";
    }
}