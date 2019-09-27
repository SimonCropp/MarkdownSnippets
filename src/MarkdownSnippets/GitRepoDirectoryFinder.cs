using System.IO;
using System;
using System.Runtime.CompilerServices;

namespace MarkdownSnippets
{
    public static class GitRepoDirectoryFinder
    {
        public static string FindForFilePath([CallerFilePath] string sourceFilePath = "")
        {
            Guard.FileExists(sourceFilePath, nameof(sourceFilePath));
            var directory = Path.GetDirectoryName(sourceFilePath);
            return FindForDirectory(directory);
        }

        public static string FindForDirectory(string directory)
        {
            Guard.DirectoryExists(directory, nameof(directory));
            if (!TryFind(directory, out var rootDirectory))
            {
                throw new Exception("Could not find git repository directory");
            }

            return rootDirectory;
        }

        public static bool IsInGitRepository(string directory)
        {
            Guard.DirectoryExists(directory, nameof(directory));
            if (TryFind(directory, out _))
            {
                return true;
            }

            return false;
        }

        public static bool TryFind(string directory, out string path)
        {
            Guard.DirectoryExists(directory, nameof(directory));

            do
            {
                if (Directory.Exists(Path.Combine(directory, ".git")))
                {
                    path = directory;
                    return true;
                }

                var parent = Directory.GetParent(directory);
                if (parent == null)
                {
                    path = "";
                    return false;
                }

                directory = parent.FullName;
            } while (true);
        }
    }
}