using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownSnippets
{
    public class SnippetFileFinder
    {
        DirectoryFilter? directoryFilter;

        public SnippetFileFinder(DirectoryFilter? directoryFilter = null)
        {
            this.directoryFilter = directoryFilter;
        }

        public bool IncludeDirectory(string directoryPath)
        {
            Guard.DirectoryExists(directoryPath, nameof(directoryPath));
            var suffix = Path.GetFileName(directoryPath);
            if (suffix.StartsWith("."))
            {
                return false;
            }

            if (DirectoryExclusions.ShouldExcludeDirectory(suffix))
            {
                return false;
            }

            if (directoryFilter == null)
            {
                return true;
            }

            return directoryFilter(directoryPath);
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
            var files = new List<string>();
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