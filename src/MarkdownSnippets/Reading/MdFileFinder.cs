using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownSnippets
{
    public class MdFileFinder
    {
        DocumentConvention convention;
        ShouldIncludeDirectory shouldIncludeDirectory;
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

        public MdFileFinder(DocumentConvention convention, ShouldIncludeDirectory shouldIncludeDirectory, IEnumerable<string>? documentExtensions = null)
        {
            this.convention = convention;
            this.shouldIncludeDirectory = shouldIncludeDirectory;
            this.documentExtensions = BuildDefaultExtensions(documentExtensions);
        }

        bool IncludeDirectory(string directoryPath)
        {
            return shouldIncludeDirectory(directoryPath);
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