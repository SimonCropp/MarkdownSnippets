using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownSnippets
{
    public class DirectorySnippetExtractor
    {
        int maxWidth;
        SnippetFileFinder fileFinder;

        public DirectorySnippetExtractor(
            DirectoryFilter? directoryFilter = null,
            int maxWidth = int.MaxValue)
        {
            Guard.AgainstNegativeAndZero(maxWidth, nameof(maxWidth));
            this.maxWidth = maxWidth;
            fileFinder = new SnippetFileFinder(directoryFilter);
        }

        public ReadSnippets ReadSnippets(params string[] directories)
        {
            Guard.AgainstNull(directories, nameof(directories));
            var files = fileFinder.FindFiles(directories).ToList();
            var snippets = files
                .SelectMany(file => Read(file, maxWidth))
                .ToList();
            return new ReadSnippets(snippets, files);
        }

        static IEnumerable<Snippet> Read(string file, int maxWidth)
        {
            using var reader = File.OpenText(file);
            return FileSnippetExtractor.Read(reader, file, maxWidth).ToList();
        }
    }
}