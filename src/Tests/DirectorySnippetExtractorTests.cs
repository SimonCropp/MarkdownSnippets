using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarkdownSnippets;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class DirectorySnippetExtractorTests
{
    [Fact]
    public Task Case()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "DirectorySnippetExtractor/Case");
        DirectorySnippetExtractor extractor = new();
        var snippets = extractor.ReadSnippets(directory);
        AssertCaseInsensitive(snippets.Lookup);

        return Verifier.Verify(snippets);
    }

    static void AssertCaseInsensitive(IReadOnlyDictionary<string, IReadOnlyList<Snippet>> dictionary)
    {
        Assert.True(dictionary.ContainsKey("Snippet"));
        Assert.True(dictionary.ContainsKey("snippet"));
    }

    [Fact]
    public Task Nested()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "DirectorySnippetExtractor/Nested");
        DirectorySnippetExtractor extractor = new();
        var snippets = extractor.ReadSnippets(directory);
        return Verifier.Verify(snippets);
    }

    [Fact]
    public Task Simple()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "DirectorySnippetExtractor/Simple");
        DirectorySnippetExtractor extractor = new();
        var snippets = extractor.ReadSnippets(directory);
        return Verifier.Verify(snippets);
    }

    [Fact]
    public Task VerifyLambdasAreCalled()
    {
        ConcurrentBag<string> directories = new();
        var targetDirectory = Path.Combine(AssemblyLocation.CurrentDirectory,
            "DirectorySnippetExtractor/VerifyLambdasAreCalled");
        DirectorySnippetExtractor extractor = new(
            shouldIncludeDirectory: path =>
            {
                directories.Add(path);
                return true;
            });
        extractor.ReadSnippets(targetDirectory);
        return Verifier.Verify(directories.OrderBy(file => file));
    }
}