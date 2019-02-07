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
            if (!TryFind(directory, out var rootDirectory))
            {
                throw new Exception("Could not find git repository directory");
            }

            return rootDirectory;
        }

        public static bool IsInGitRepository(string targetDirectory)
        {
            Guard.DirectoryExists(targetDirectory,nameof(targetDirectory));
            if (TryFind(targetDirectory, out _))
            {
                return true;
            }

            return false;
        }

        public static bool TryFind(string targetDirectory, out string path)
        {
            Guard.DirectoryExists(targetDirectory,nameof(targetDirectory));

            do
            {
                if (Directory.Exists(Path.Combine(targetDirectory, ".git")))
                {
                    path = targetDirectory;
                    return true;
                }

                var parent = Directory.GetParent(targetDirectory);
                if (parent == null)
                {
                    path = null;
                    return false;
                }

                targetDirectory = parent.FullName;
            } while (true);
        }
    }
}