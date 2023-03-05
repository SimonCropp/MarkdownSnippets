#if NET5_0_OR_GREATER
using System.IO.Enumeration;
#endif
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
#if NETSTANDARD
        FindFiles(targetDirectory);
#else
        ProcessFiles(targetDirectory);
        var directoryEnumerator = new FileSystemEnumerable<FileSystemInfo>(
            targetDirectory,
            (ref FileSystemEntry entry) => entry.ToFileSystemInfo(),
            new(){RecurseSubdirectories = true,})
        {
            ShouldIncludePredicate = (ref FileSystemEntry entry) =>
                entry.IsDirectory &&
                directoryIncludes(entry.ToFullPath()),
            ShouldRecursePredicate = (ref FileSystemEntry entry) =>  directoryIncludes(entry.ToFullPath())
        };
        foreach (var item in directoryEnumerator)
        {
            ProcessFiles(item.FullName);
        }
#endif
        return (
            snippetFiles.OrderBy(_ => _).ToList(),
            mdFiles.OrderBy(_ => _).ToList(),
            includeFiles.OrderBy(_ => _).ToList(),
            allFiles.OrderBy(_ => _).ToList());
    }

    void FindFiles(string directory)
    {
        ProcessFiles(directory);

        foreach (var subDirectory in Directory.EnumerateDirectories(directory)
                     .Where(path => directoryIncludes(path.AsSpan())))
        {
            FindFiles(subDirectory);
        }
    }

    void ProcessFiles(string directory)
    {
        var scanForMarkdown = markdownDirectoryIncludes(directory.AsSpan());
        var scanForSnippets = snippetDirectoryIncludes(directory.AsSpan());
        if (scanForSnippets && scanForMarkdown)
        {
            foreach (var file in EnumerateFiles(directory))
            {
                allFiles.Add(file);

                if (file.IsMdFile())
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
                         .Where(_ => !_.IsMdFile()))
            {
                allFiles.Add(file);
                snippetFiles.Add(file);
            }

            return;
        }

        if (scanForMarkdown)
        {
            foreach (var file in EnumerateFiles(directory)
                         .Where(Paths.IsMdFile))
            {
                allFiles.Add(file);
                ProcessMarkdown(file);
            }
        }
    }

    static IEnumerable<string> EnumerateFiles(string directory) =>
        Directory.EnumerateFiles(directory)
            .Where(ShouldInclude);

    void ProcessMarkdown(string file)
    {
        if (file.IsIncludeMdFile())
        {
            includeFiles.Add(file);
            return;
        }

        if (convention != DocumentConvention.SourceTransform)
        {
            mdFiles.Add(file);
            return;
        }

        if (file.IsSourceMdFile())
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