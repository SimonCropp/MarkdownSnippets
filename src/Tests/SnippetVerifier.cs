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
        StringBuilder stringBuilder = new();
        using StringReader reader = new(markdownContent);
        using StringWriter writer = new(stringBuilder);
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
        return new(
            convention: convention,
            snippets: snippets.ToDictionary(),
            includes: includes,
            appendSnippets: SimpleSnippetMarkdownHandling.Append,
            snippetSourceFiles: snippetSourceFiles,
            tocLevel: 2,
            writeHeader: false,
            rootDirectory: "c:/root",
            validateContent: true);
    }

    public static async Task<string> Verify(
        DocumentConvention convention,
        string markdownContent,
        List<Snippet>? snippets = null,
        IReadOnlyList<string>? snippetSourceFiles = null,
        IReadOnlyList<Include>? includes = null,
        [CallerFilePath] string sourceFile = "")
    {

        var markdownProcessor = BuildProcessor(convention, snippets, snippetSourceFiles, includes);
        StringBuilder stringBuilder = new();
        using StringReader reader = new(markdownContent);
        await using StringWriter writer = new(stringBuilder);
        var processResult = markdownProcessor.Apply(reader, writer, "sourceFile");
        var result = stringBuilder.ToString();
        var output = new
        {
            processResult.MissingSnippets,
            processResult.UsedSnippets,
            result
        };
        await Verifier.Verify(output, null, sourceFile);
        return result;
    }
}