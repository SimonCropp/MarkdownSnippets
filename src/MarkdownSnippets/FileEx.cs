using System;
using System.IO;

static class FileEx
{
    public static FileStream OpenRead(string path)
    {
        return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }
    public static string GetRelativePath(string file, string directory)
    {
        var fileUri = new Uri(file);
        // Folders must end in a slash
        if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            directory += Path.DirectorySeparatorChar;
        }
        var directoryUri = new Uri(directory);
        return Uri.UnescapeDataString(directoryUri.MakeRelativeUri(fileUri).ToString().Replace('/', Path.DirectorySeparatorChar));
    }

    public static void ClearReadOnly(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }
        var fileInfo = new FileInfo(path)
        {
            IsReadOnly = false
        };
    }

    public static void MakeReadOnly(string path)
    {
        var fileInfo = new FileInfo(path)
        {
            IsReadOnly = true
        };
    }
}