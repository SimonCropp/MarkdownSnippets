using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownSnippets;

class IncludeFileFinder
{
    ShouldIncludeDirectory shouldIncludeDirectory;

    public IncludeFileFinder(ShouldIncludeDirectory shouldIncludeDirectory)
    {
        this.shouldIncludeDirectory = shouldIncludeDirectory;
    }


    static bool IncludeFile(string path)
    {
        var fileName = Path.GetFileName(path);
        if (fileName.StartsWith("."))
        {
            return false;
        }

        return fileName.EndsWith(".include.md");
    }

    public List<string> FindFiles(params string[] directoryPaths)
    {
        Guard.AgainstNull(directoryPaths, nameof(directoryPaths));
        List<string> files = new();
        foreach (var directoryPath in directoryPaths)
        {
            Guard.DirectoryExists(directoryPath, nameof(directoryPath));
            FindFiles(Path.GetFullPath(directoryPath), files);
        }

        return files;
    }

    void FindFiles(string directoryPath, List<string> files)
    {
        foreach (var file in Directory.EnumerateFiles(directoryPath)
            .Where(IncludeFile))
        {
            files.Add(file);
        }

        foreach (var subDirectory in Directory.EnumerateDirectories(directoryPath)
            .Where(IncludeDirectory))
        {
            FindFiles(subDirectory, files);
        }
    }

    bool IncludeDirectory(string directoryPath)
    {
        return shouldIncludeDirectory(directoryPath);
    }
}