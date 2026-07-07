using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using MarkdownSnippets;

// Tier 3 benchmarks for the follow-up perf pass:
//   #11 GetLanguageFromPath span+ToLowerInvariant  -> GetLanguageFromPathBenchmark
//   #12 Line indentation / toc-check allocations    -> IndentedMarkdownApplyBenchmark
//
// Each builds a fresh subject per invocation to match real-run lifetimes.

/// <summary>
/// Exercises #11: GetLanguageFromPath is called for every file during discovery (including
/// binaries), and again per snippet source file. The old GetExtension + TrimStart('.') +
/// ToLowerInvariant chain allocated three strings per call; the span form allocates one.
/// </summary>
[MemoryDiagnoser]
public class GetLanguageFromPathBenchmark
{
    string[] paths = null!;

    [GlobalSetup]
    public void Setup() =>
        paths =
        [
            "src/MarkdownSnippets/Reading/FileSnippetExtractor.cs",
            "C:/Code/Repo/Some/Deep/Nested/Folder/File.CS",
            "readme.md",
            "data.JSON",
            "Program.vb",
            "build.props",
            "noextension",
            "archive.tar.gz",
            "styles.CSS",
            "script.PY",
        ];

    [Benchmark(Description = "GetLanguageFromPath x10 varied paths")]
    public int Resolve()
    {
        var total = 0;
        foreach (var path in paths)
        {
            total += FileSnippetExtractor.GetLanguageFromPath(path).Length;
        }

        return total;
    }
}

/// <summary>
/// Exercises #12: an indentation-heavy markdown document. Every indented line previously
/// allocated (a) a trimmed string for the `line.Current.TrimStart() == "toc"` check and
/// (b) a leading-whitespace substring in the Line constructor - even though only snippet
/// lines ever read LeadingWhitespace. Both are now avoided for non-snippet indented lines.
/// </summary>
[MemoryDiagnoser]
public class IndentedMarkdownApplyBenchmark
{
    Dictionary<string, IReadOnlyList<Snippet>> snippets = null!;
    string markdown = null!;
    string tempDir = null!;

    const int Blocks = 200;

    [GlobalSetup]
    public void Setup()
    {
        tempDir = Path.Combine(Path.GetTempPath(), "msnip-bench-indent-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        snippets = new();
        for (var s = 0; s < 10; s++)
        {
            var key = $"snippet_{s}";
            snippets[key] = [Snippet.Build(1, 3, "line1\nline2\nline3", key, "cs", null, null)];
        }

        var sb = new StringBuilder();
        for (var block = 0; block < Blocks; block++)
        {
            sb.Append("## Heading ").Append(block).Append('\n');
            sb.Append('\n');
            sb.Append("Some plain paragraph prose describing the section in detail.\n");
            sb.Append("    indented list or code line that has leading whitespace here\n");
            sb.Append("      more deeply indented content line with text\n");
            sb.Append("- a bullet item\n");
            sb.Append('\n');
            if (block % 20 == 0)
            {
                sb.Append("snippet: snippet_").Append(block % 10).Append('\n');
                sb.Append('\n');
            }
        }

        markdown = sb.ToString();
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

    [Benchmark(Description = "Apply over indentation-heavy doc (200 blocks)")]
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
