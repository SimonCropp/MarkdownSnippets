using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownSnippets
{
    public static class DirectoryExclusions
    {
        public static bool ShouldExcludeDirectory(string path)
        {
            return ExcludedDirectories.Any(func => func(path));
        }

        public static List<Func<string, bool>> ExcludedDirectories { get; set; } = new()
        {
            path =>
            {
                var suffix = Path.GetFileName(path).ToLowerInvariant();
                return suffix.StartsWith("_") ||
                       suffix.StartsWith(".") ||
                       suffix == "bin" ||
                       suffix == "obj";
            }
        };
    }
}