using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownSnippets;

class FileFinder
{
    string rootDirectory;
    DocumentConvention convention;
    ShouldIncludeDirectory shouldIncludeDirectory;
    List<string> files = new();
    List<string> documentExtensions;

    public FileFinder(string rootDirectory, DocumentConvention convention, ShouldIncludeDirectory shouldIncludeDirectory, List<string> documentExtensions)
    {
        this.rootDirectory = rootDirectory;
        this.convention = convention;
        this.shouldIncludeDirectory = shouldIncludeDirectory;
        this.documentExtensions = documentExtensions;
    }


    public List<string> FindFiles()
    {
        FindFiles(rootDirectory);
        return files;
    }

    void FindFiles(string directory)
    {
        foreach (var file in Directory.EnumerateFiles(directory)
            .Where(IncludeFile))
        {
            var extension = Path.GetExtension(file);
            if (extension == string.Empty)
            {
                continue;
            }

            if (SnippetFileExclusions.IsBinary(extension))
            {
                continue;
            }

            files.Add(file);
        }

        foreach (var subDirectory in Directory.EnumerateDirectories(directory)
            .Where(path => shouldIncludeDirectory(path)))
        {
            FindFiles(subDirectory);
        }
    }

    bool IncludeFile(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        if (extension == string.Empty)
        {
            return false;
        }

        return !SnippetFileExclusions.IsBinary(extension);
    }
}