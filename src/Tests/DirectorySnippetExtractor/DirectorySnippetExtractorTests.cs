using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownSnippets;
using ObjectApproval;
using Xunit;

public class DirectorySnippetExtractorTests : TestBase
{
    [Fact]
    public void Case()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "DirectorySnippetExtractor/Case");
        var extractor = new DirectorySnippetExtractor();
        var snippets = extractor.ReadSnippets(directory);
        AssertCaseInsensitive(snippets.Lookup);
        ObjectApprover.VerifyWithJson(snippets, Scrubber.Scrub);
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
        ObjectApprover.VerifyWithJson(components, Scrubber.Scrub);
    }

    [Fact]
    public void Simple()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "DirectorySnippetExtractor/Simple");
        var extractor = new DirectorySnippetExtractor();
        var components = extractor.ReadSnippets(directory);
        ObjectApprover.VerifyWithJson(components, Scrubber.Scrub);
    }

    [Fact]
    public void VerifyLambdasAreCalled()
    {
        var directories = new ConcurrentBag<CapturedDirectory>();
        var files = new ConcurrentBag<CapturedFile>();
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
            },
            fileFilter: path =>
            {
                var capture = new CapturedFile
                {
                    Path = path
                };
                files.Add(capture);
                return true;
            }
        );
        extractor.ReadSnippets(targetDirectory);
        result.Files = files.OrderBy(file => file.Path).ToList();
        result.Directories = directories.OrderBy(file => file.Path).ToList();
        ObjectApprover.VerifyWithJson(result, Scrubber.Scrub);
    }

    public class TestResult
    {
        public List<CapturedDirectory> Directories;
        public List<CapturedFile> Files;
    }

    public class CapturedDirectory
    {
        public string Path;
    }

    public class CapturedFile
    {
        public string Path;
    }
}