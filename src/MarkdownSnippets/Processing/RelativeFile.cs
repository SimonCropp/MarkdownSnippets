using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static class RelativeFile
{
    static bool InnerFind(IReadOnlyList<string> allFiles, string rootDirectory, string key, string? relativePath, string? linePath, out string path)
    {
        if (!key.Contains("."))
        {
            path = null!;
            return false;
        }

        var relativeToRoot = Path.Combine(rootDirectory, key);
        if (File.Exists(relativeToRoot))
        {
            path = relativeToRoot;
            return true;
        }

        var documentDirectory = Path.GetDirectoryName(relativePath);
        if (documentDirectory != null)
        {
            var relativeToDocument = Path.Combine(rootDirectory, documentDirectory.Trim('/', '\\'), key);
            if (File.Exists(relativeToDocument))
            {
                path = relativeToDocument;
                return true;
            }
        }

        var lineDirectory = Path.GetDirectoryName(linePath);
        if (lineDirectory != null)
        {
            var relativeToLine = Path.Combine(lineDirectory, key);
            if (File.Exists(relativeToLine))
            {
                path = relativeToLine;
                return true;
            }
        }

        if (File.Exists(key))
        {
            path = Path.GetFullPath(key);
            return true;
        }

        var suffix = FileEx.PrependSlash(key);
        var endWith = allFiles
            .FirstOrDefault(x => x.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
        if (endWith != null)
        {
            path = endWith;
            return true;
        }

        path = null!;
        return false;
    }

    public static bool Find(IReadOnlyList<string> allFiles, string rootDirectory, string key, string? relativePath, string? linePath, out string path)
    {
        if (InnerFind(allFiles, rootDirectory, key, relativePath, linePath, out path))
        {
            path = FileEx.FixFileCapitalization(path);
            return true;
        }

        return false;
    }
}