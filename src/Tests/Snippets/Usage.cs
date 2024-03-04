using MarkdownSnippets;
// ReSharper disable UnusedVariable

class Usage
{
    static void ReadingFiles()
    {
        #region ReadingFilesSimple

        var files = Directory.EnumerateFiles(@"C:\path", "*.cs", SearchOption.AllDirectories);

        var snippets = FileSnippetExtractor.Read(files);

        #endregion
    }

    static void DirectoryMarkdownProcessorRun()
    {
        #region DirectoryMarkdownProcessorRun

        var processor = new DirectoryMarkdownProcessor(
            "targetDirectory",
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.Run();

        #endregion
    }

    static void DirectoryMarkdownProcessorRunMaxWidth()
    {
        #region DirectoryMarkdownProcessorRunMaxWidth

        var processor = new DirectoryMarkdownProcessor(
            "targetDirectory",
            maxWidth: 80,
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.Run();

        #endregion
    }
}