using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CaptureSnippets
{
    public class DirectorySnippetExtractor
    {
        FileFinder fileFinder;

        public DirectorySnippetExtractor(DirectoryFilter directoryFilter = null, FileFilter fileFilter = null)
        {
            fileFinder = new FileFinder(directoryFilter, fileFilter);
        }

        public ReadSnippets ReadSnippets(string directory)
        {
            var snippets = fileFinder.FindFiles(directory)
                .SelectMany(Read)
                .ToList();
            return new ReadSnippets(snippets);
        }

        public ReadSnippets ReadSnippets(params string[] directories)
        {
            Guard.AgainstNull(directories, nameof(directories));
            var snippets = fileFinder.FindFiles(directories)
                .SelectMany(Read)
                .ToList();
            return new ReadSnippets(snippets);
        }

        private static IEnumerable<Snippet> Read(string file)
        {
            using (var reader = File.OpenText(file))
            {
                return FileSnippetExtractor.Read(reader, file).ToList();
            }
        }
    }
}