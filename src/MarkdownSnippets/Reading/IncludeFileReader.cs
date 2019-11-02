using System;
using System.Collections.Generic;
using System.IO;
using MarkdownSnippets;

static class IncludeFileReader
{
    public static Func<string, Include> ReadIncludes(IEnumerable<string> files)
    {
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