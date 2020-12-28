﻿using System.IO;
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

        DirectoryMarkdownProcessor processor = new(
            "targetDirectory",
            shouldIncludeDirectory: _ => true);
        processor.Run();

        #endregion
    }

    void DirectoryMarkdownProcessorRunMaxWidth()
    {
        #region DirectoryMarkdownProcessorRunMaxWidth

        DirectoryMarkdownProcessor processor = new(
            "targetDirectory",
            maxWidth: 80,
            shouldIncludeDirectory: _ => true);
        processor.Run();

        #endregion
    }
}