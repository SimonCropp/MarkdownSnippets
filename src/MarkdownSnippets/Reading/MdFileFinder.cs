using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownSnippets
{
    public class MdFileFinder
    {
        DirectoryFilter? directoryFilter;
        List<string> documentExtensions;

        public MdFileFinder(DirectoryFilter? directoryFilter = null, IEnumerable<string>? documentExtensions = null)
        {
            this.directoryFilter = directoryFilter;
            if (documentExtensions == null)
            {
                this.documentExtensions = new List<string> {"md"};
            }
            else
            {
                this.documentExtensions = documentExtensions.ToList();
            }
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
            foreach (var extension in documentExtensions)
            {
                files.AddRange(FileEx.FindFiles(directoryPath, $"*.source.{extension}"));
            }

            foreach (var subDirectory in Directory.EnumerateDirectories(directoryPath)
                .Where(IncludeDirectory))
            {
                FindFiles(subDirectory, files);
            }
        }
    }
}