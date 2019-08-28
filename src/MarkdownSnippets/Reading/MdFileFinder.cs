using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownSnippets
{
    public class MdFileFinder
    {
        DirectoryFilter directoryFilter;

        public MdFileFinder(DirectoryFilter directoryFilter = null)
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
            try
            {
                foreach (var file in Directory.EnumerateFiles(directoryPath, "*.source.md"))
                {
                    files.Add(file);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }

            foreach (var subDirectory in Directory.EnumerateDirectories(directoryPath)
                .Where(IncludeDirectory))
            {
                FindFiles(subDirectory, files);
            }
        }
    }
}