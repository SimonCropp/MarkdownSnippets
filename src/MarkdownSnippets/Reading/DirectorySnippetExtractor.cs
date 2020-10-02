using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownSnippets
{
    public class DirectorySnippetExtractor
    {
        string newLine;
        int maxWidth;
        SnippetFileFinder fileFinder;

        public DirectorySnippetExtractor(
            DirectoryFilter? directoryFilter = null,
            int maxWidth = int.MaxValue,
            string newLine = "\n")
        {
            Guard.AgainstNull(newLine, nameof(newLine));
            Guard.AgainstNegativeAndZero(maxWidth, nameof(maxWidth));
            this.newLine = newLine;
            this.maxWidth = maxWidth;
            fileFinder = new SnippetFileFinder(directoryFilter);
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

        IEnumerable<Snippet> Read(string file)
        {
            using var reader = File.OpenText(file);
            return FileSnippetExtractor.Read(reader, file, maxWidth, newLine).ToList();
        }
    }
}