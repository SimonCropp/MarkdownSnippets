using System.Collections.Generic;
using System.IO;
using MarkdownSnippets;

class Usage
{
    void ReadingFiles()
    {
        #region ReadingFilesSimple

        var files = Directory.EnumerateFiles(@"C:\path", "*.cs", SearchOption.AllDirectories);

        var snippets = FileSnippetExtractor.Read(files);

        #endregion
    }

    void DirectoryMarkdownProcessorRun()
    {
        #region DirectoryMarkdownProcessorRun

        var processor = new DirectoryMarkdownProcessor("targetDirectory");
        processor.Run();

        #endregion
    }

    void DirectoryMarkdownProcessorRunMaxWidth()
    {
        #region DirectoryMarkdownProcessorRunMaxWidth

        var processor = new DirectoryMarkdownProcessor(
            "targetDirectory",
            maxWidth: 80);
        processor.Run();

        #endregion
    }

    void ReadingDirectory()
    {
        #region ReadingDirectorySimple

        // extract snippets from files
        var snippetExtractor = new DirectorySnippetExtractor(
            // all directories except bin and obj
            directoryFilter: dirPath => !dirPath.EndsWith("bin") &&
                                        !dirPath.EndsWith("obj"));
        var snippets = snippetExtractor.ReadSnippets(@"C:\path");

        #endregion
    }

    void Basic()
    {
        #region markdownProcessingSimple

        var directory = @"C:\path";

        // extract snippets from files
        var snippetExtractor = new DirectorySnippetExtractor();
        var snippets = snippetExtractor.ReadSnippets(directory);

        // extract includes from files
        var includeFinder = new IncludeFinder();
        var includes = includeFinder.ReadIncludes(directory);

        // Merge with some markdown text
        var markdownProcessor = new MarkdownProcessor(
            snippets: snippets.Lookup,
            includes: includes,
            appendSnippetGroup: SimpleSnippetMarkdownHandling.AppendGroup,
            snippetSourceFiles: new List<string>(),
            tocLevel: 2,
            writeHeader: true,
            rootDirectory: directory);

        var path = @"C:\path\inputMarkdownFile.md";
        using var reader = File.OpenText(path);
        using var writer = File.CreateText(@"C:\path\outputMarkdownFile.md");
        var result = markdownProcessor.Apply(reader, writer, path);
        // snippets that the markdown file expected but did not exist in the input snippets
        var missingSnippets = result.MissingSnippets;

        // snippets that the markdown file used
        var usedSnippets = result.UsedSnippets;

        #endregion
    }
}