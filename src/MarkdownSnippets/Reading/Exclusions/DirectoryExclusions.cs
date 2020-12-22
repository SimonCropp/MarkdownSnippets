using System;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownSnippets
{
    public static class DirectoryExclusions
    {
        public static bool ShouldExcludeDirectory(string suffix)
        {
            suffix = suffix.ToLowerInvariant();
            if (ExcludedDirectories.Any(func => func(suffix)))
            {
                return true;
            }
            return ExcludedDirectorySuffixes.Contains(suffix);
        }

        public static List<string> ExcludedDirectorySuffixes { get; set; } = new();

        public static List<Func<string, bool>> ExcludedDirectories { get; set; } = new()
        {
            s => s.StartsWith("_"),
            s => s.StartsWith("."),
            s => s == "bin",
            s => s == "obj"
        };
    }
}