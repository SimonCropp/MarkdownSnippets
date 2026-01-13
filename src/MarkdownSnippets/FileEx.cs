static class FileEx
{
    public static string FixFileCapitalization(string file)
    {
        var fileName = Path.GetFileName(file);
        var directory = Path.GetDirectoryName(file);
        if (string.IsNullOrEmpty(directory))
        {
            directory = ".";
        }

        var filePaths = Directory.GetFiles(directory, fileName, SearchOption.TopDirectoryOnly);
        if (filePaths.Length == 0)
        {
            throw new FileNotFoundException($"Could not find file: {file}");
        }

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

        new FileInfo(path)
        {
            IsReadOnly = false
        };
    }

    public static void MakeReadOnly(string path) =>
        new FileInfo(path)
        {
            IsReadOnly = true
        };
}