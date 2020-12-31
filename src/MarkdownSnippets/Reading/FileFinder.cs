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
        ProcessFiles(rootDirectory);
        foreach (var subDirectory in Directory.EnumerateDirectories(rootDirectory)
            .Where(path => shouldIncludeDirectory(path)))
        {
            FindFiles(subDirectory);
        }

        return (snippetFiles, mdFiles, includeFiles, allFiles);
    }

    void FindFiles(string directory)
    {
        ProcessFiles(directory);

        foreach (var subDirectory in Directory.EnumerateDirectories(directory)
            .Where(path => shouldIncludeDirectory(path)))
        {
            FindFiles(subDirectory);
        }
    }

    void ProcessFiles(string directory)
    {
        foreach (var file in Directory.EnumerateFiles(directory))
        {
            var extension = Path.GetExtension(file);
            if (extension == string.Empty)
            {
                continue;
            }

            extension = extension.Substring(1);
            if (SnippetFileExclusions.IsBinary(extension))
            {
                continue;
            }

            allFiles.Add(file);

            if (extension == "md")
            {
                if (file.EndsWith(".include.md"))
                {
                    includeFiles.Add(file);
                    continue;
                }

                if (convention == DocumentConvention.SourceTransform)
                {
                    if (file.EndsWith(".source.md"))
                    {
                        mdFiles.Add(file);
                    }
                }
                else
                {
                    mdFiles.Add(file);
                }

                continue;
            }

            snippetFiles.Add(file);
        }
    }
}