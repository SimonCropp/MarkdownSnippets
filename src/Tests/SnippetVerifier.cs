using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MarkdownSnippets;
using VerifyXunit;

static class SnippetVerifier
{
    public static Task VerifySnippets(
        string markdownContent,
        List<Snippet> availableSnippets,
        List<string> snippetSourceFiles,
        IReadOnlyList<Include>? includes = null,
        [CallerFilePath] string sourceFile = "")
    {
        includes ??= Array.Empty<Include>();

        var markdownProcessor = new MarkdownProcessor(
            convention: DocumentConvention.SourceTransform,
            snippets: availableSnippets.ToDictionary(),
            appendSnippetGroup: SimpleSnippetMarkdownHandling.AppendGroup,
            snippetSourceFiles: snippetSourceFiles,
            tocLevel: 2,
            writeHeader: false,
            validateContent: true,
            includes: includes,
            rootDirectory: "c:/root");
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
        return Verifier.Verify(output, null, sourceFile);
    }
}