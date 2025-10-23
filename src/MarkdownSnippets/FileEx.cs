static class FileEx
{
    public static string FixFileCapitalization(string file)
    {
        var fileName = Path.GetFileName(file);
        var directory = Path.GetDirectoryName(file);
        var filePaths = Directory.GetFiles(directory!, fileName, SearchOption.TopDirectoryOnly);
        return filePaths[0];
    }

    public static FileStream OpenRead(string path) =>
        new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

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

    public static IEnumerable<string> FindFiles(string directory, string pattern)
    {
        var files = new List<string>();
        try
        {
            files.AddRange(Directory.EnumerateFiles(directory, pattern));
        }
        catch (UnauthorizedAccessException)
        {
        }

        return files;
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