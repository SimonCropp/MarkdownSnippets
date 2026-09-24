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

            // ai agents
            ".claude" or
            ".cursor" or
            ".windsurf" or
            ".gemini" or

            // package cache
            "packages" or
            "node_modules" or

            // build output
            "dist" or
            ".angular" or
            "bin" or
            "obj")
        {
            return true;
        }

        // Cache Directory Tagging spec (https://bford.info/cachedir/). Cargo writes one into its target
        // directory, which can hold files cargo keeps locked while a build is running
        if (File.Exists(Path.Combine(path, "CACHEDIR.TAG")))
        {
            return true;
        }

        var directory = new DirectoryInfo(path);
        return directory.Attributes.HasFlag(FileAttributes.Hidden);
    }
}