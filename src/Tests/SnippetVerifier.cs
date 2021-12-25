using MarkdownSnippets;

static class SnippetVerifier
{
    public static Task VerifyThrows(
        DocumentConvention convention,
        string markdownContent,
        IReadOnlyList<Snippet>? snippets = null,
        IReadOnlyList<string>? snippetSourceFiles = null,
        IReadOnlyList<Include>? includes = null,
        [CallerFilePath] string sourceFile = "")
    {
        var processor = BuildProcessor(convention, snippets, snippetSourceFiles, includes);
        StringBuilder builder = new();
        using StringReader reader = new(markdownContent);
        using StringWriter writer = new(builder);
        return Throws(() => processor.Apply(reader, writer, "sourceFile"), null, sourceFile);
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
            targetDirectory: "c:/root",
            validateContent: true,
            allFiles: new List<string>());
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