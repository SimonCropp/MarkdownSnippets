using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownSnippets;

class FileFinder
{
    string targetDirectory;
    DocumentConvention convention;
    ShouldIncludeDirectory directoryIncludes;
    ShouldIncludeDirectory markdownDirectoryIncludes;
    ShouldIncludeDirectory snippetDirectoryIncludes;
    List<string> snippetFiles = new();
    List<string> mdFiles = new();
    List<string> allFiles = new();
    List<string> includeFiles = new();

    public FileFinder(
        string targetDirectory,
        DocumentConvention convention,
        ShouldIncludeDirectory directoryIncludes,
        ShouldIncludeDirectory markdownDirectoryIncludes,
        ShouldIncludeDirectory snippetDirectoryIncludes)
    {
        this.targetDirectory = targetDirectory;
        this.convention = convention;
        this.directoryIncludes = directoryIncludes;
        this.markdownDirectoryIncludes = markdownDirectoryIncludes;
        this.snippetDirectoryIncludes = snippetDirectoryIncludes;
    }

    public (List<string> snippetFiles, List<string> mdFiles, List<string> includeFiles, List<string> allFiles) FindFiles()
    {
        ProcessFiles(targetDirectory);
        foreach (var subDirectory in Directory.EnumerateDirectories(targetDirectory)
            .Where(path => directoryIncludes(path)))
        {
            FindFiles(subDirectory);
        }

        return (snippetFiles, mdFiles, includeFiles, allFiles);
    }

    void FindFiles(string directory)
    {
        ProcessFiles(directory);

        foreach (var subDirectory in Directory.EnumerateDirectories(directory)
            .Where(path => directoryIncludes(path)))
        {
            FindFiles(subDirectory);
        }
    }

    void ProcessFiles(string directory)
    {
        var scanForMarkdown = markdownDirectoryIncludes(directory);
        var scanForSnippets = snippetDirectoryIncludes(directory);
        if (scanForSnippets && scanForMarkdown)
        {
            foreach (var file in EnumerateFiles(directory))
            {
                allFiles.Add(file);

                if (file.EndsWith(".md"))
                {
                    ProcessMarkdown(file);

                    continue;
                }

                snippetFiles.Add(file);
            }
            return;
        }

        if (scanForSnippets)
        {
            foreach (var file in EnumerateFiles(directory)
                .Where(x=>!x.EndsWith(".md")))
            {
                allFiles.Add(file);
                snippetFiles.Add(file);
            }
            return;
        }

        if (scanForMarkdown)
        {
            foreach (var file in EnumerateFiles(directory)
                .Where(x=>x.EndsWith(".md")))
            {
                allFiles.Add(file);
                ProcessMarkdown(file);
            }
        }
    }

    static IEnumerable<string> EnumerateFiles(string directory)
    {
        return Directory.EnumerateFiles(directory)
            .Where(ShouldInclude);
    }

    void ProcessMarkdown(string file)
    {
        if (file.EndsWith(".include.md"))
        {
            includeFiles.Add(file);
            return;
        }

        if (convention != DocumentConvention.SourceTransform)
        {
            mdFiles.Add(file);
            return;
        }

        if (file.EndsWith(".source.md"))
        {
            mdFiles.Add(file);
        }
    }

    static bool ShouldInclude(string file)
    {
        var extension = Path.GetExtension(file);
        if (extension == string.Empty)
        {
            return false;
        }

        extension = extension.Substring(1);
        return !SnippetFileExclusions.IsBinary(extension);
    }
}