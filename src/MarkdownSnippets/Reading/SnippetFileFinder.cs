using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownSnippets
{
    public class SnippetFileFinder
    {
        ShouldIncludeDirectory shouldIncludeDirectory;

        public SnippetFileFinder(ShouldIncludeDirectory shouldIncludeDirectory)
        {
            this.shouldIncludeDirectory = shouldIncludeDirectory;
        }

        bool IncludeDirectory(string directoryPath)
        {
            return shouldIncludeDirectory(directoryPath);
        }

        static bool IncludeFile(string path)
        {
            var fileName = Path.GetFileName(path);
            if (fileName.StartsWith("."))
            {
                return false;
            }

            var extension = Path.GetExtension(fileName);
            if (extension == string.Empty)
            {
                return false;
            }

            if (SnippetFileExclusions.ShouldExcludeExtension(extension.Substring(1)))
            {
                return false;
            }

            return true;
        }

        public List<string> FindFiles(params string[] directoryPaths)
        {
            Guard.AgainstNull(directoryPaths, nameof(directoryPaths));
            List<string> files = new();
            foreach (var directoryPath in directoryPaths)
            {
                Guard.DirectoryExists(directoryPath, nameof(directoryPath));
                FindFiles(Path.GetFullPath(directoryPath), files);
            }

            return files;
        }

        void FindFiles(string directoryPath, List<string> files)
        {
            foreach (var file in Directory.EnumerateFiles(directoryPath)
                .Where(IncludeFile))
            {
                files.Add(file);
            }

            foreach (var subDirectory in Directory.EnumerateDirectories(directoryPath)
                .Where(IncludeDirectory))
            {
                FindFiles(subDirectory, files);
            }
        }
    }
}