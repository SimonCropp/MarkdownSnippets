using System;
using System.Collections.Generic;
using System.IO;

namespace MarkdownSnippets
{
    public static class IncludeFileReader
    {
        public static Func<string, string[]> ReadIncludes(IEnumerable<string> files)
        {
            var dictionary = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in files)
            {
                var key = Path.GetFileName(file).Replace(".include.md", "");
                if (dictionary.ContainsKey(key))
                {
                    throw new Exception($"Duplicate include: {key}");
                }

                dictionary[key] = File.ReadAllLines(file);
            }

            return key =>
            {
                if (dictionary.TryGetValue(key, out var lines))
                {
                    return lines;
                }

                throw new Exception("Missing key: " + key);
            };
        }
    }
}