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
    public static Task VerifyThrows<T>(
        DocumentConvention convention,
        string markdownContent,
        IReadOnlyList<Snippet>? snippets = null,
        IReadOnlyList<string>? snippetSourceFiles = null,
        IReadOnlyList<Include>? includes = null,
        [CallerFilePath] string sourceFile = "")
    where T: Exception
    {
        var markdownProcessor = BuildProcessor(convention, snippets, snippetSourceFiles, includes);
        var stringBuilder = new StringBuilder();
        using var reader = new StringReader(markdownContent);
        using var writer = new StringWriter(stringBuilder);
        return Verifier.Throws(() => markdownProcessor.Apply(reader, writer, "sourceFile"), null, sourceFile);
    }

    static MarkdownProcessor BuildProcessor(
        DocumentConvention convention,
        IReadOnlyList<Snippet>? snippets,
        IReadOnlyList<string>? snippetSourceFiles,
        IReadOnlyList<Include>? includes)
    {
        includes ??= Array.Empty<Include>();
        snippets ??= Array.Empty<Snippet>();
        snippetSourceFiles ??= Array.Empty<string>();
        return new MarkdownProcessor(
            convention: convention,
            snippets: snippets.ToDictionary(),
            appendSnippets: SimpleSnippetMarkdownHandling.Append,
            snippetSourceFiles: snippetSourceFiles,
            tocLevel: 2,
            writeHeader: false,
            validateContent: true,
            includes: includes,
            rootDirectory: "c:/root");
    }

    public static Task Verify(
        DocumentConvention convention,
        string markdownContent,
        List<Snippet>? snippets = null,
        IReadOnlyList<string>? snippetSourceFiles = null,
        IReadOnlyList<Include>? includes = null,
        [CallerFilePath] string sourceFile = "")
    {

        var markdownProcessor = BuildProcessor(convention, snippets, snippetSourceFiles, includes);
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