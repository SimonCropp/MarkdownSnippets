using System.Collections.Generic;

namespace MarkdownSnippets
{
    public static class DirectoryExclusions
    {
        public static bool ShouldExcludeDirectory(string suffix)
        {
            suffix = suffix.ToLowerInvariant();
            return ExcludedDirectorySuffixes.Contains(suffix);
        }

        public static List<string> ExcludedDirectorySuffixes { get; set; } = new List<string>
        {
            "bin",
            "obj"
        };
    }
}