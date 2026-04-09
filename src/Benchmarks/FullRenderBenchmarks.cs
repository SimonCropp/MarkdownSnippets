using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using MarkdownSnippets;

[MemoryDiagnoser]
public class FullRenderBenchmarks
{
    string repoRoot = null!;
    Dictionary<string, IReadOnlyList<Snippet>> snippetLookup = null!;
    string markdownInput = null!;

    static bool DirectoryFilter(string path) =>
        !path.Contains("IncludeFileFinder") &&
        !path.Contains("DirectoryMarkdownProcessor") &&
        !DefaultDirectoryExclusions.ShouldExcludeDirectory(path);

    static bool SnippetDirectoryFilter(string path) =>
        DirectoryFilter(path) &&
        !path.Contains("badsnippets");

    [GlobalSetup]
    public void Setup()
    {
        repoRoot = GitRepoDirectoryFinder.FindForFilePath();

        // pre-load snippets for the in-memory MarkdownProcessor benchmark
        var processor = new DirectoryMarkdownProcessor(
            targetDirectory: repoRoot,
            directoryIncludes: DirectoryFilter,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: SnippetDirectoryFilter,
            scanForMdFiles: false,
            tocLevel: 1);
        snippetLookup = processor.Snippets
            .Where(s => !s.IsInError)
            .GroupBy(s => s.Key)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<Snippet>) g.ToList());

        // build a synthetic markdown doc that references real snippet keys
        var sb = new StringBuilder();
        sb.AppendLine("# Benchmark Document");
        sb.AppendLine();
        foreach (var key in snippetLookup.Keys.Take(50))
        {
            sb.AppendLine($"snippet: {key}");
            sb.AppendLine();
        }

        markdownInput = sb.ToString();
    }

    /// <summary>
    /// End-to-end: file discovery, snippet extraction, markdown processing, and file writing.
    /// </summary>
    [Benchmark(Description = "Full repo render")]
    public void FullRepoRender()
    {
        var processor = new DirectoryMarkdownProcessor(
            targetDirectory: repoRoot,
            directoryIncludes: DirectoryFilter,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: SnippetDirectoryFilter,
            tocLevel: 1,
            tocExcludes: ["Icon", "Credits", "Release Notes"]);
        processor.Run();
    }

    /// <summary>
    /// Constructor only: file discovery + snippet extraction (no markdown processing).
    /// </summary>
    [Benchmark(Description = "File discovery + snippet extraction")]
    public DirectoryMarkdownProcessor FileDiscoveryAndSnippetExtraction() =>
        new(
            targetDirectory: repoRoot,
            directoryIncludes: DirectoryFilter,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: SnippetDirectoryFilter,
            scanForMdFiles: false,
            tocLevel: 1);

    /// <summary>
    /// In-memory markdown processing with pre-loaded snippets, no file I/O.
    /// </summary>
    [Benchmark(Description = "MarkdownProcessor.Apply (50 snippets)")]
    public string MarkdownProcessorApply()
    {
        var processor = new MarkdownProcessor(
            convention: DocumentConvention.SourceTransform,
            snippets: snippetLookup,
            includes: [],
            appendSnippets: SimpleSnippetMarkdownHandling.Append,
            snippetSourceFiles: [],
            tocLevel: 2,
            writeHeader: false,
            targetDirectory: repoRoot,
            validateContent: false,
            allFiles: []);

        return processor.Apply(markdownInput, "benchmark.source.md");
    }
}
