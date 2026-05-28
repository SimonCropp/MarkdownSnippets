using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using MarkdownSnippets;

// Targeted benchmarks for the four optimizations described in the perf review:
//   #1 ReadNonStartEndLines caching        -> WholeFileSnippetBenchmark
//   #2 Snippet.Value.Lines() caching       -> SnippetAsIncludeBenchmark
//   #3 validateContent hoist               -> ValidateContentBenchmark
//   #4 FilesToSnippets lookup table        -> FilesToSnippetsLookupBenchmark
//
// Each benchmark constructs a fresh MarkdownProcessor per invocation so that any
// caches we add live for exactly one document - matching the lifetime in real
// runs (one MarkdownProcessor per DirectoryMarkdownProcessor.Run, applied to many
// markdown files).

/// <summary>
/// Exercises #1 ReadNonStartEndLines caching: a markdown that references the same
/// whole-file snippet many times. Without a cache, every reference re-reads the
/// source file, scans every line through StartEndTester, and rebuilds the body.
/// </summary>
[MemoryDiagnoser]
public class WholeFileSnippetBenchmark
{
    string tempDir = null!;
    List<string> snippetSourceFiles = null!;
    List<string> allFiles = null!;
    string markdown = null!;

    const int Repetitions = 100;
    const int LinesPerFile = 200;
    const string FileName = "whole_file.cs";

    [GlobalSetup]
    public void Setup()
    {
        tempDir = Path.Combine(Path.GetTempPath(), "msnip-bench-whole-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        var filePath = Path.Combine(tempDir, FileName);
        var body = new StringBuilder();
        for (var i = 0; i < LinesPerFile; i++)
        {
            body.Append("    // ").Append(FileName).Append(" line ").Append(i).Append('\n');
        }
        File.WriteAllText(filePath, body.ToString());

        snippetSourceFiles = [filePath];
        allFiles = [filePath];

        var md = new StringBuilder();
        md.Append("# Whole-file repeat benchmark\n\n");
        for (var i = 0; i < Repetitions; i++)
        {
            md.Append("snippet: ").Append(FileName).Append("\n\n");
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

    [Benchmark(Description = "Whole-file snippet x100 (same file)")]
    public string Apply()
    {
        var processor = new MarkdownProcessor(
            convention: DocumentConvention.SourceTransform,
            snippets: new Dictionary<string, IReadOnlyList<Snippet>>(),
            includes: [],
            appendSnippets: SimpleSnippetMarkdownHandling.Append,
            snippetSourceFiles: snippetSourceFiles,
            tocLevel: 2,
            writeHeader: false,
            targetDirectory: tempDir,
            validateContent: false,
            allFiles: allFiles);
        return processor.Apply(markdown, "bench.source.md");
    }
}

/// <summary>
/// Exercises #2 Snippet.Value.Lines() caching: a markdown that uses the same
/// snippet as an include many times. Each include resolves through
/// IncludeProcessor and triggers Snippet.Value.Lines(), which currently splits
/// the body string on every call.
/// </summary>
[MemoryDiagnoser]
public class SnippetAsIncludeBenchmark
{
    Dictionary<string, IReadOnlyList<Snippet>> snippets = null!;
    string markdown = null!;
    string tempDir = null!;

    const int Repetitions = 100;
    const int LinesPerSnippet = 200;
    const string Key = "my_snippet";

    [GlobalSetup]
    public void Setup()
    {
        tempDir = Path.Combine(Path.GetTempPath(), "msnip-bench-include-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        var body = new StringBuilder();
        for (var i = 0; i < LinesPerSnippet; i++)
        {
            body.Append("snippet body line ").Append(i);
            if (i < LinesPerSnippet - 1)
            {
                body.Append('\n');
            }
        }
        var snippet = Snippet.Build(
            startLine: 1,
            endLine: LinesPerSnippet,
            value: body.ToString(),
            key: Key,
            language: "cs",
            path: null,
            expressiveCode: null);
        snippets = new Dictionary<string, IReadOnlyList<Snippet>>
        {
            [Key] = [snippet]
        };

        var md = new StringBuilder();
        md.Append("# Snippet-as-include benchmark\n\n");
        for (var i = 0; i < Repetitions; i++)
        {
            md.Append("include: ").Append(Key).Append("\n\n");
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

    [Benchmark(Description = "Snippet-as-include x100 (same snippet)")]
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
/// Exercises #3 validationExcludes hoist: a large markdown processed with
/// validateContent=true. validationExcludes.Any(relativePath.Contains) runs once
/// per line even though relativePath is constant for the whole document.
/// </summary>
[MemoryDiagnoser]
public class ValidateContentBenchmark
{
    string markdown = null!;
    string tempDir = null!;

    const int LineCount = 5000;

    [GlobalSetup]
    public void Setup()
    {
        tempDir = Path.Combine(Path.GetTempPath(), "msnip-bench-validate-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        var md = new StringBuilder();
        md.Append("# Validation benchmark\n\n");
        for (var i = 0; i < LineCount; i++)
        {
            md.Append("Plain paragraph content line ").Append(i).Append(" with nothing special.\n");
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

    [Benchmark(Description = "validateContent over 5000 lines")]
    public string Apply()
    {
        var processor = new MarkdownProcessor(
            convention: DocumentConvention.SourceTransform,
            snippets: new Dictionary<string, IReadOnlyList<Snippet>>(),
            includes: [],
            appendSnippets: SimpleSnippetMarkdownHandling.Append,
            snippetSourceFiles: [],
            tocLevel: 2,
            writeHeader: false,
            targetDirectory: tempDir,
            validateContent: true,
            allFiles: []);
        return processor.Apply(markdown, "bench.source.md");
    }
}

/// <summary>
/// Exercises #4 FilesToSnippets lookup: many snippet source files and many
/// unique unknown keys. The current implementation does an EndsWith linear scan
/// over snippetSourceFiles for every key not in the dictionary.
/// </summary>
[MemoryDiagnoser]
public class FilesToSnippetsLookupBenchmark
{
    string tempDir = null!;
    List<string> snippetSourceFiles = null!;
    List<string> allFiles = null!;
    string markdown = null!;

    const int FileCount = 1000;
    const int Lookups = 100;

    [GlobalSetup]
    public void Setup()
    {
        tempDir = Path.Combine(Path.GetTempPath(), "msnip-bench-files-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        snippetSourceFiles = new(FileCount);
        for (var i = 0; i < FileCount; i++)
        {
            var path = Path.Combine(tempDir, $"src_file_{i:D4}.cs");
            // single short line: ReadNonStartEndLines stays cheap, isolating the scan cost
            File.WriteAllText(path, $"// file {i}\n");
            snippetSourceFiles.Add(path);
        }
        allFiles = [..snippetSourceFiles];

        // spread the chosen keys across the file list so an EndsWith scan averages O(N/2)
        var step = FileCount / Lookups;
        var md = new StringBuilder();
        md.Append("# Linear scan benchmark\n\n");
        for (var i = 0; i < Lookups; i++)
        {
            var fileIndex = ((i + 1) * step) - 1;
            md.Append("snippet: src_file_").Append(fileIndex.ToString("D4")).Append(".cs\n\n");
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

    [Benchmark(Description = "FilesToSnippets scan: 1000 files, 100 unique keys")]
    public string Apply()
    {
        var processor = new MarkdownProcessor(
            convention: DocumentConvention.SourceTransform,
            snippets: new Dictionary<string, IReadOnlyList<Snippet>>(),
            includes: [],
            appendSnippets: SimpleSnippetMarkdownHandling.Append,
            snippetSourceFiles: snippetSourceFiles,
            tocLevel: 2,
            writeHeader: false,
            targetDirectory: tempDir,
            validateContent: false,
            allFiles: allFiles);
        return processor.Apply(markdown, "bench.source.md");
    }
}
