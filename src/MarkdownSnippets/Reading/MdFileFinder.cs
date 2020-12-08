using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownSnippets
{
    public class MdFileFinder
    {
        DocumentConvention convention;
        DirectoryFilter? directoryFilter;
        List<string> documentExtensions;

        static List<string> defaultExtensions = new() {"md"};

        internal static List<string> BuildDefaultExtensions(IEnumerable<string>? documentExtensions = null)
        {
            if (documentExtensions == null)
            {
                return defaultExtensions;
            }

            var list = documentExtensions.ToList();
            if (list.Any())
            {
                return list;
            }

            return defaultExtensions;
        }

        public MdFileFinder(DocumentConvention convention, DirectoryFilter? directoryFilter = null, IEnumerable<string>? documentExtensions = null)
        {
            this.convention = convention;
            this.directoryFilter = directoryFilter;
            this.documentExtensions = BuildDefaultExtensions(documentExtensions);
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
            foreach (var extension in documentExtensions)
            {
                IEnumerable<string> findFiles;
                if (convention == DocumentConvention.SourceTransform)
                {
                    findFiles = FileEx.FindFiles(directoryPath, $"*.source.{extension}");
                }
                else
                {
                    findFiles = FileEx.FindFiles(directoryPath, $"*.{extension}")
                        .Where(x => !x.Contains(".include."));
                }

                files.AddRange(findFiles);
            }

            foreach (var subDirectory in Directory.EnumerateDirectories(directoryPath)
                .Where(IncludeDirectory))
            {
                FindFiles(subDirectory, files);
            }
        }
    }
}