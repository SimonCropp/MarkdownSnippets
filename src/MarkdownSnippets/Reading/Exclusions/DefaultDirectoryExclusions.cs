using System.IO;

namespace MarkdownSnippets
{
    public static class DefaultDirectoryExclusions
    {
        public static bool ShouldExcludeDirectory(string path)
        {
            var suffix = Path.GetFileName(path).ToLowerInvariant();
            return suffix.StartsWith("_") ||
                   suffix.StartsWith(".") ||
                   suffix is
                       "packages" or
                       "node_modules" or
                       "bin" or
                       "obj";
        }
    };
}