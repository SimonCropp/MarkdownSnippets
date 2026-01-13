static class FileEx
{
    public static string FixFileCapitalization(string file)
    {
        var fileName = Path.GetFileName(file);
        var directory = Path.GetDirectoryName(file);
        var filePaths = Directory.GetFiles(directory!, fileName, SearchOption.TopDirectoryOnly);
        return filePaths[0];
    }

    public static string GetRelativePath(string file, string directory)
    {
        var fileUri = new Uri(file);
        // Folders must end in a slash
        if (!directory.EndsWith(Path.DirectorySeparatorChar))
        {
            directory += Path.DirectorySeparatorChar;
        }

        var directoryUri = new Uri(directory);
        return Uri.UnescapeDataString(directoryUri.MakeRelativeUri(fileUri).ToString().Replace('/', Path.DirectorySeparatorChar));
    }

    public static string PrependSlash(string path)
    {
        if (path.StartsWith('/'))
        {
            return path;
        }

        return $"/{path}";
    }

    public static void ClearReadOnly(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        var attributes = File.GetAttributes(path);
        if ((attributes & FileAttributes.ReadOnly) != 0)
        {
            File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
        }
    }

    public static void MakeReadOnly(string path)
    {
        var attributes = File.GetAttributes(path);
        File.SetAttributes(path, attributes | FileAttributes.ReadOnly);
    }
}