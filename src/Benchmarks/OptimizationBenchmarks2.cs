using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using MarkdownSnippets;

// Tier 2/3 benchmarks for the remaining items from the perf review:
//   #5  CreateIndentedAppendLine closure removal -> ClosurePerSnippetLineBenchmark
//   #6  SnippetExtensions.ScrubPath single-pass  -> ReadSnippetsBenchmark (shared with #8)
//   #7  SnippetMarkdownHandling path normalize   -> SnippetMarkdownHandlingBenchmark
//   #8  ReadSnippets empty SnippetsInError       -> ReadSnippetsBenchmark (shared with #6)
//   #9  MarkdownProcessor ctor path normalize    -> MarkdownProcessorCtorBenchmark
//   #10 StartEndTester early-bail                -> StartEndTesterBenchmark
//
// Each benchmark builds a fresh subject per invocation to match the lifetimes used in real runs.

/// <summary>
/// Exercises #5: every "snippet:" line goes through ProcessSnippetLine -> CreateIndentedAppendLine,
/// which currently allocates a closure (state class + Action<string> delegate) on every call.
/// </summary>
[MemoryDiagnoser]
public class ClosurePerSnippetLineBenchmark
{
    Dictionary<string, IReadOnlyList<Snippet>> snippets = null!;
    string markdown = null!;
    string tempDir = null!;

    const int SnippetCount = 200;

    [GlobalSetup]
    public void Setup()
    {
        tempDir = Path.Combine(Path.GetTempPath(), "msnip-bench-closure-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        snippets = new();
        var md = new StringBuilder();
        md.Append("# Closure benchmark\n\n");
        for (var i = 0; i < SnippetCount; i++)
        {
            var key = $"key_{i:D4}";
            var snippet = Snippet.Build(
                startLine: 1,
                endLine: 1,
                value: "tiny body",
                key: key,
                language: "cs",
                path: null,
                expressiveCode: null);
            snippets[key] = [snippet];
            md.Append("snippet: ").Append(key).Append("\n\n");
        }
        markdown = md.ToString();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        try
        {
            Directory.Delete(tempDir, recursive: true);
        }
        catch
        {
        }
    }

    [Benchmark(Description = "Many snippet refs (200 dictionary hits)")]
    public string Apply()
    {
        var processor = new MarkdownProcessor(
            convention: DocumentConvention.SourceTransform,
            snippets: snippets,
            includes: [],
            appendSnippets: SimpleSnippetMarkdownHandling.Append,
            snippetSourceFiles: [],
            tocLevel: 2,
            writeHeader: false,
            targetDirectory: tempDir,
            validateContent: false,
            allFiles: []);
        return processor.Apply(markdown, "bench.source.md");
    }
}

/// <summary>
/// Exercises #6 + #8: ReadSnippets ctor calls Snippets.ToDictionary() which does
/// GroupBy + OrderBy(ScrubPath), and builds SnippetsInError via Where().Distinct().ToList()
/// even when nothing is in error. Uses repeating keys so OrderBy actually compares (and so
/// ScrubPath is exercised) inside each group.
/// </summary>
[MemoryDiagnoser]
public class ReadSnippetsBenchmark
{
    List<Snippet> snippets = null!;

    const int KeyCount = 100;
    const int SnippetsPerKey = 10;

    [GlobalSetup]
    public void Setup()
    {
        snippets = new(KeyCount * SnippetsPerKey);
        for (var k = 0; k < KeyCount; k++)
        {
            var key = $"key_{k:D4}";
            for (var i = 0; i < SnippetsPerKey; i++)
            {
                // long path with many separators - ScrubPath has real work per call
                var path = $"/some/long/nested/structure/folder_{k:D4}/sub_{i}/File_{k}_{i}.cs";
                snippets.Add(Snippet.Build(
                    startLine: 1,
                    endLine: 1,
                    value: "body",
                    key: key,
                    language: "cs",
                    path: path,
                    expressiveCode: null));
            }
        }
    }

    [Benchmark(Description = "ReadSnippets ctor: 1000 snippets, 100 keys x 10 paths")]
    public ReadSnippets BuildReadSnippets() =>
        new(snippets, []);
}

/// <summary>
/// Exercises #7: every snippet rendered through SnippetMarkdownHandling.Append calls
/// GetSupText, which does pathLocal.Replace('\\', '/') on every snippet - one allocation per
/// snippet write, even though Snippet.Path is immutable.
/// </summary>
[MemoryDiagnoser]
public class SnippetMarkdownHandlingBenchmark
{
    Dictionary<string, IReadOnlyList<Snippet>> snippets = null!;
    string markdown = null!;
    string tempDir = null!;
    SnippetMarkdownHandling handling = null!;

    const int SnippetCount = 200;

    [GlobalSetup]
    public void Setup()
    {
        tempDir = Path.Combine(Path.GetTempPath(), "msnip-bench-handling-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        snippets = new();
        var md = new StringBuilder();
        md.Append("# SnippetMarkdownHandling benchmark\n\n");
        for (var i = 0; i < SnippetCount; i++)
        {
            var key = $"key_{i:D4}";
            // use a path under tempDir with backslashes so Replace has real work
            var path = Path.Combine(tempDir, "src", $"folder_{i}", $"File_{i}.cs");
            var snippet = Snippet.Build(
                startLine: 1,
                endLine: 1,
                value: "body",
                key: key,
                language: "cs",
                path: path,
                expressiveCode: null);
            snippets[key] = [snippet];
            md.Append("snippet: ").Append(key).Append("\n\n");
        }
        markdown = md.ToString();
        handling = new SnippetMarkdownHandling(tempDir, LinkFormat.GitHub, omitSnippetLinks: false);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        try
        {
            Directory.Delete(tempDir, recursive: true);
        }
        catch
        {
        }
    }

    [Benchmark(Description = "SnippetMarkdownHandling.Append over 200 snippets")]
    public string Apply()
    {
        var processor = new MarkdownProcessor(
            convention: DocumentConvention.SourceTransform,
            snippets: snippets,
            includes: [],
            appendSnippets: handling.Append,
            snippetSourceFiles: [],
            tocLevel: 2,
            writeHeader: false,
            targetDirectory: tempDir,
            validateContent: false,
            allFiles: []);
        return processor.Apply(markdown, "bench.source.md");
    }
}

/// <summary>
/// Exercises #9: MarkdownProcessor ctor does allFiles.Select(_ => _.Replace('\\', '/')).ToList()
/// and the same for snippetSourceFiles. With 1000 paths in each list this is 2000 string allocations
/// (one per path that contains a backslash) plus two List<string> allocations.
/// </summary>
[MemoryDiagnoser]
public class MarkdownProcessorCtorBenchmark
{
    List<string> allFiles = null!;
    List<string> snippetSourceFiles = null!;
    Dictionary<string, IReadOnlyList<Snippet>> emptySnippets = null!;
    string tempDir = null!;

    const int FileCount = 1000;

    [GlobalSetup]
    public void Setup()
    {
        tempDir = Path.Combine(Path.GetTempPath(), "msnip-bench-ctor-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        allFiles = new(FileCount);
        snippetSourceFiles = new(FileCount);
        for (var i = 0; i < FileCount; i++)
        {
            // Windows-style paths with backslashes so Replace actually allocates a new string.
            var path = Path.Combine(tempDir, $"sub_{i / 100}", $"file_{i:D4}.cs");
            allFiles.Add(path);
            snippetSourceFiles.Add(path);
        }

        emptySnippets = new();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        try
        {
            Directory.Delete(tempDir, recursive: true);
        }
        catch
        {
        }
    }

    [Benchmark(Description = "MarkdownProcessor ctor: 1000 files")]
    public MarkdownProcessor BuildProcessor() =>
        new(
            convention: DocumentConvention.SourceTransform,
            snippets: emptySnippets,
            includes: [],
            appendSnippets: SimpleSnippetMarkdownHandling.Append,
            snippetSourceFiles: snippetSourceFiles,
            tocLevel: 2,
            writeHeader: false,
            targetDirectory: tempDir,
            validateContent: false,
            allFiles: allFiles);
}

/// <summary>
/// Exercises #10: FileSnippetExtractor.Read goes line-by-line through StartEndTester.IsStart for
/// every line of every source file. Files with no snippet markers still pay the IndexOf scan
/// on every line. Uses many small source files of plain code (no markers).
/// </summary>
[MemoryDiagnoser]
public class StartEndTesterBenchmark
{
    string tempDir = null!;
    List<string> filePaths = null!;

    const int FileCount = 50;
    const int LinesPerFile = 500;

    [GlobalSetup]
    public void Setup()
    {
        tempDir = Path.Combine(Path.GetTempPath(), "msnip-bench-startend-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        filePaths = new(FileCount);
        for (var f = 0; f < FileCount; f++)
        {
            var path = Path.Combine(tempDir, $"src_{f:D3}.cs");
            var body = new StringBuilder();
            for (var i = 0; i < LinesPerFile; i++)
            {
                // varied lines like real source code; none contain begin-snippet / end-snippet / #region
                var lineText = (i % 5) switch
                {
                    0 => "    var x = 1 + 2;",
                    1 => "    Console.WriteLine(value);",
                    2 => "    }",
                    3 => "    public int Property { get; set; }",
                    _ => "    // ordinary comment text",
                };
                body.Append(lineText).Append('\n');
            }
            File.WriteAllText(path, body.ToString());
            filePaths.Add(path);
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        try
        {
            Directory.Delete(tempDir, recursive: true);
        }
        catch
        {
        }
    }

    [Benchmark(Description = "FileSnippetExtractor.Read 50 files x 500 plain lines")]
    public List<Snippet> ReadAll() =>
        FileSnippetExtractor.Read(filePaths).ToList();
}
