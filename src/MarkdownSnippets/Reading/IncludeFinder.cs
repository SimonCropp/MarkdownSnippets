using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownSnippets
{
    public class IncludeFinder
    {
        IncludeFileFinder fileFinder;

        public IncludeFinder(DirectoryFilter? directoryFilter = null)
        {
            fileFinder = new IncludeFileFinder(directoryFilter);
        }

        public IReadOnlyList<Include> ReadIncludes(params string[] directories)
        {
            Guard.AgainstNull(directories, nameof(directories));
            var files = fileFinder.FindFiles(directories).ToList();
            var dictionary = new Dictionary<string, Include>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in files)
            {
                var key = Path.GetFileName(file).Replace(".include.md", "");
                if (dictionary.ContainsKey(key))
                {
                    throw new Exception($"Duplicate include: {key}");
                }

                dictionary[key] = Include.Build(key, File.ReadAllLines(file), file);
            }

            return dictionary.Values.ToList();
        }
    }
}