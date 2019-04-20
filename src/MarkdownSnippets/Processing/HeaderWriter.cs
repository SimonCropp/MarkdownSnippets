using System.IO;

static class HeaderWriter
{
    public static void WriteHeader(string sourceFile, string targetDirectory, TextWriter writer)
    {
        var relativeSource = sourceFile
            .ReplaceCaseless(targetDirectory, "")
            .Replace('\\', '/');
        writer.WriteLine(@"<!--");
        writer.WriteLine(@"This file was generate by MarkdownSnippets.");
        writer.WriteLine($@"Source File: {relativeSource}");
        writer.WriteLine(@"To change this file edit the source file and then re-run the generation using either the dotnet global tool (https://github.com/SimonCropp/MarkdownSnippets#githubmarkdownsnippets) or using the api (https://github.com/SimonCropp/MarkdownSnippets#running-as-a-unit-test).");
        writer.WriteLine(@"-->");
    }
}