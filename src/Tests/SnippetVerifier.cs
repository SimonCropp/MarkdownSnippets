static class SnippetVerifier
{
    public static ProcessResult Apply(string markdownContent, MarkdownProcessor processor)
    {
        var builder = new StringBuilder();
        using var reader = new StringReader(markdownContent);
        using var writer = new StringWriter(builder);
        return processor.Apply(reader, writer, "sourceFile");
    }

    public static MarkdownProcessor BuildProcessor(
        DocumentConvention convention,
        IReadOnlyList<Snippet>? snippets,
        IReadOnlyList<string>? snippetSourceFiles,
        IReadOnlyList<Include>? includes)
    {
        includes ??= [];
        snippets ??= [];
        snippetSourceFiles ??= [];
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

    public record RenderResult(IReadOnlyList<MissingSnippet> MissingSnippets, IReadOnlyList<Snippet> UsedSnippets, string result);

    public static RenderResult Render(DocumentConvention convention, string markdownContent, List<Snippet>? snippets, IReadOnlyList<string>? snippetSourceFiles, IReadOnlyList<Include>? includes)
    {
        var markdownProcessor = BuildProcessor(convention, snippets, snippetSourceFiles, includes);
        var stringBuilder = new StringBuilder();
        using var reader = new StringReader(markdownContent);
        // ReSharper disable once UseAwaitUsing
        using var writer = new StringWriter(stringBuilder);
        var processResult = markdownProcessor.Apply(reader, writer, "sourceFile");
        var result = stringBuilder.ToString();
        return new(processResult.MissingSnippets, processResult.UsedSnippets, result);
    }
}