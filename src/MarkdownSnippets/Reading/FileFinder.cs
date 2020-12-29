using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownSnippets;

class FileFinder
{
    string rootDirectory;
    DocumentConvention convention;
    ShouldIncludeDirectory shouldIncludeDirectory;
    List<string> snippetFiles = new();
    List<string> mdFiles = new();
    List<string> allFiles = new();
    List<string> includeFiles = new();

    public FileFinder(string rootDirectory, DocumentConvention convention, ShouldIncludeDirectory shouldIncludeDirectory)
    {
        this.rootDirectory = rootDirectory;
        this.convention = convention;
        this.shouldIncludeDirectory = shouldIncludeDirectory;
    }

    public (List<string> snippetFiles, List<string> mdFiles, List<string> includeFiles, List<string> allFiles) FindFiles()
    {
        FindFiles(rootDirectory);
        return (snippetFiles, mdFiles, includeFiles,allFiles);
    }

    void FindFiles(string directory)
    {
        foreach (var file in Directory.EnumerateFiles(directory))
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

            var fixedFile = file.Replace('\\', '/');
            allFiles.Add(fixedFile);

            if (file.EndsWith(".include.md"))
            {
                includeFiles.Add(fixedFile);
                continue;
            }

            if (extension == ".md")
            {
                if (convention == DocumentConvention.SourceTransform)
                {
                    if (fixedFile.EndsWith(".source.md"))
                    {
                        mdFiles.Add(fixedFile);
                    }
                }
                else
                {
                    mdFiles.Add(fixedFile);
                }

                continue;
            }

            snippetFiles.Add(fixedFile);
        }

        foreach (var subDirectory in Directory.EnumerateDirectories(directory)
            .Where(path => shouldIncludeDirectory(path)))
        {
            FindFiles(subDirectory);
        }
    }
}