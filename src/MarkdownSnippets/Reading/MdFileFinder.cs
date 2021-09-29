namespace MarkdownSnippets
{
    public class MdFileFinder
    {
        DocumentConvention convention;
        ShouldIncludeDirectory shouldIncludeDirectory;

        public MdFileFinder(DocumentConvention convention, ShouldIncludeDirectory shouldIncludeDirectory)
        {
            this.convention = convention;
            this.shouldIncludeDirectory = shouldIncludeDirectory;
        }

        bool IncludeDirectory(string directoryPath)
        {
            return shouldIncludeDirectory(directoryPath);
        }

        public List<string> FindFiles(params string[] directoryPaths)
        {
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
                IEnumerable<string> findFiles;
                if (convention == DocumentConvention.SourceTransform)
                {
                    findFiles = FileEx.FindFiles(directoryPath, "*.source.md");
                }
                else
                {
                    findFiles = FileEx.FindFiles(directoryPath, "*.md")
                        .Where(x => !x.Contains(".include."));
                }

                files.AddRange(findFiles);

            foreach (var subDirectory in Directory.EnumerateDirectories(directoryPath)
                .Where(IncludeDirectory))
            {
                FindFiles(subDirectory, files);
            }
        }
    }
}