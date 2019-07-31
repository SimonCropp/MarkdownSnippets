using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownSnippets;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class DirectorySnippetExtractorTests : TestBase
{
    [Fact]
    public void Case()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "DirectorySnippetExtractor/Case");
        var extractor = new DirectorySnippetExtractor();
        var snippets = extractor.ReadSnippets(directory);
        AssertCaseInsensitive(snippets.Lookup);
        ObjectApprover.Verify(snippets, Scrubber.Scrub);
    }

    static void AssertCaseInsensitive(IReadOnlyDictionary<string, IReadOnlyList<Snippet>> dictionary)
    {
        Assert.True(dictionary.ContainsKey("Snippet"));
        Assert.True(dictionary.ContainsKey("snippet"));
    }

    [Fact]
    public void Nested()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "DirectorySnippetExtractor/Nested");
        var extractor = new DirectorySnippetExtractor();
        var components = extractor.ReadSnippets(directory);
        ObjectApprover.Verify(components, Scrubber.Scrub);
    }

    [Fact]
    public void Simple()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "DirectorySnippetExtractor/Simple");
        var extractor = new DirectorySnippetExtractor();
        var components = extractor.ReadSnippets(directory);
        ObjectApprover.Verify(components, Scrubber.Scrub);
    }

    [Fact]
    public void VerifyLambdasAreCalled()
    {
        var directories = new ConcurrentBag<CapturedDirectory>();
        var targetDirectory = Path.Combine(AssemblyLocation.CurrentDirectory,
            "DirectorySnippetExtractor/VerifyLambdasAreCalled");
        var result = new TestResult();
        var extractor = new DirectorySnippetExtractor(
            directoryFilter: path =>
            {
                var capture = new CapturedDirectory
                {
                    Path = path
                };
                directories.Add(capture);
                return true;
            }
        );
        extractor.ReadSnippets(targetDirectory);
        result.Directories = directories.OrderBy(file => file.Path).ToList();
        ObjectApprover.Verify(result, Scrubber.Scrub);
    }

    public class TestResult
    {
        public List<CapturedDirectory> Directories;
    }

    public class CapturedDirectory
    {
        public string Path;
    }

    public DirectorySnippetExtractorTests(ITestOutputHelper output) :
        base(output)
    {
    }
}