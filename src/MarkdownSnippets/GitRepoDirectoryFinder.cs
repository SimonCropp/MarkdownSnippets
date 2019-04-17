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

        public static string GetHash(string targetDirectory)
        {
            Guard.DirectoryExists(targetDirectory,nameof(targetDirectory));
            var head = ReadHead(targetDirectory);
            var @ref = Path.Combine(targetDirectory, ".git", head);
            return ReadFirstLine(@ref);
        }

        static string ReadHead(string targetDirectory)
        {
            var head = Path.Combine(targetDirectory, ".git", "HEAD");
            var line = ReadFirstLine(head);
            return line.Replace("ref: ", "");
        }

        static string ReadFirstLine(string head)
        {
            using (var stream = FileEx.OpenRead(head))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadLine();
            }
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