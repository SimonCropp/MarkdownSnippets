using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MarkdownSnippets;

static class SnippetVerifier
{
    public static void Verify(
        string markdownContent,
        List<Snippet> availableSnippets,
        List<string> snippetSourceFiles,
        IReadOnlyList<Include>? includes=null)
    {
        if (includes == null)
        {
            includes = Array.Empty<Include>();
        }
        var markdownProcessor = new MarkdownProcessor(
            snippets: availableSnippets.ToDictionary(),
            appendSnippetGroup: SimpleSnippetMarkdownHandling.AppendGroup,
            snippetSourceFiles: snippetSourceFiles,
            tocLevel: 2,
            writeHeader: true,
            includes: includes);
        var stringBuilder = new StringBuilder();
        using var reader = new StringReader(markdownContent);
        using var writer = new StringWriter(stringBuilder);
        var processResult = markdownProcessor.Apply(reader, writer, "sourceFile");
        var output = new
        {
            processResult.MissingSnippets,
            processResult.UsedSnippets,
            content = stringBuilder.ToString()
        };
        ObjectApprover.Verify(output, Scrub);
    }

    static string Scrub(string value)
    {
        var gitDir = GitRepoDirectoryFinder.FindForFilePath().Replace('\\', '/');
        return value
            .Replace("\\r\\n", "\r\n")
            .Replace(gitDir, "");
    }
}