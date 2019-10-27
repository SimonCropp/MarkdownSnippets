using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class DirectorySnippetExtractorTests :
    XunitApprovalBase
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
        var directories = new ConcurrentBag<string>();
        var targetDirectory = Path.Combine(AssemblyLocation.CurrentDirectory,
            "DirectorySnippetExtractor/VerifyLambdasAreCalled");
        var extractor = new DirectorySnippetExtractor(
            directoryFilter: path =>
            {
                directories.Add(path);
                return true;
            }
        );
        extractor.ReadSnippets(targetDirectory);
        ObjectApprover.Verify(directories.OrderBy(file => file), Scrubber.Scrub);
    }

    public DirectorySnippetExtractorTests(ITestOutputHelper output) :
        base(output)
    {
    }
}