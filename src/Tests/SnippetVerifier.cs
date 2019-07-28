using System.Collections.Generic;
using System.IO;
using System.Text;
using MarkdownSnippets;
using ObjectApproval;

static class SnippetVerifier
{
    public static void Verify(string markdownContent, List<Snippet> availableSnippets, List<string> snippetSourceFiles)
    {
        var markdownProcessor = new MarkdownProcessor(
            snippets: availableSnippets.ToDictionary(),
            appendSnippetGroup: SimpleSnippetMarkdownHandling.AppendGroup,
            snippetSourceFiles: snippetSourceFiles,
            writeHeader: true,
            tocLevel:2);
        var stringBuilder = new StringBuilder();
        using (var reader = new StringReader(markdownContent))
        using (var writer = new StringWriter(stringBuilder))
        {
            var processResult = markdownProcessor.Apply(reader, writer, null);
            var output = new
            {
                processResult.MissingSnippets,
                processResult.UsedSnippets,
                content = stringBuilder.ToString()
            };
            ObjectApprover.VerifyWithJson(output, Scrub);
        }
    }

    static string Scrub(string value)
    {
        var gitDir = GitRepoDirectoryFinder.FindForFilePath().Replace('\\', '/');
        return value
            .Replace("\\r\\n", "\r\n")
            .Replace(gitDir, "");
    }
}