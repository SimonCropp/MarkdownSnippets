using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownSnippets
{
    public class IncludeFinder
    {
        IncludeFileFinder fileFinder;

        public IncludeFinder(ShouldIncludeDirectory? shouldIncludeDirectory = null)
        {
            fileFinder = new(shouldIncludeDirectory);
        }

        public IReadOnlyList<Include> ReadIncludes(params string[] directories)
        {
            Guard.AgainstNull(directories, nameof(directories));
            var files = fileFinder.FindFiles(directories).ToList();
            Dictionary<string, Include> dictionary = new(StringComparer.OrdinalIgnoreCase);
            foreach (var file in files)
            {
                var key = Path.GetFileName(file).Replace(".include.md", "");
                if (dictionary.ContainsKey(key))
                {
                    throw new($"Duplicate include: {key}");
                }

                dictionary[key] = Include.Build(key, File.ReadAllLines(file), file);
            }

            return dictionary.Values.ToList();
        }
    }
}