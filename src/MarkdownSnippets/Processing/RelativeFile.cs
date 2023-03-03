using System.Diagnostics.CodeAnalysis;

static class RelativeFile
{
    static bool InnerFind(IReadOnlyList<string> allFiles, string targetDirectory, string key, string? relativePath, string? linePath, out string path)
    {
        if (!key.Contains('.'))
        {
            path = null!;
            return false;
        }

        var relativeToRoot = Path.Combine(targetDirectory, key);
        if (File.Exists(relativeToRoot))
        {
            path = relativeToRoot;
            return true;
        }

        var documentDirectory = Path.GetDirectoryName(relativePath);
        if (documentDirectory != null)
        {
            var relativeToDocument = Path.Combine(targetDirectory, documentDirectory.Trim('/', '\\'), key);
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
            .FirstOrDefault(_ => _.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
        if (endWith != null)
        {
            path = endWith;
            return true;
        }

        path = null!;
        return false;
    }

    public static bool Find(
        IReadOnlyList<string> allFiles,
        string targetDirectory,
        string key,
        string? relativePath,
        string? linePath,
        [NotNullWhen(true)] out string? path)
    {
        if (!InnerFind(allFiles, targetDirectory, key, relativePath, linePath, out path))
        {
            return false;
        }

        path = FileEx.FixFileCapitalization(path);
        return true;
    }
}