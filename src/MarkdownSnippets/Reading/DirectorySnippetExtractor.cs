using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownSnippets
{
    public class DirectorySnippetExtractor
    {
        FileFinder fileFinder;

        public DirectorySnippetExtractor(DirectoryFilter directoryFilter = null, FileFilter fileFilter = null)
        {
            fileFinder = new FileFinder(directoryFilter, fileFilter);
        }

        public ReadSnippets ReadSnippets(params string[] directories)
        {
            Guard.AgainstNull(directories, nameof(directories));
            var files = fileFinder.FindFiles(directories).ToList();
            var snippets = files
                .SelectMany(Read)
                .ToList();
            return new ReadSnippets(snippets, files);
        }

        static IEnumerable<Snippet> Read(string file)
        {
            using (var reader = File.OpenText(file))
            {
                return FileSnippetExtractor.Read(reader, file).ToList();
            }
        }
    }
}