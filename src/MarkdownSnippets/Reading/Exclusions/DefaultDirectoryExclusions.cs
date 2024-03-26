namespace MarkdownSnippets;

public static class DefaultDirectoryExclusions
{
    public static bool ShouldExcludeDirectory(string path)
    {
        var suffix = Path
            .GetFileName(path)
            .ToLowerInvariant();
        if (suffix is
            // source control
            ".git" or

            // ide temp files
            ".vs" or
            ".vscode" or
            ".idea" or

            // angular cache
            ".angular" or

            // package cache
            "packages" or
            "node_modules" or

            // build output
            "bin" or
            "obj")
        {
            return true;
        }

        var directory = new DirectoryInfo(path);
        return directory.Attributes.HasFlag(FileAttributes.Hidden);
    }
}