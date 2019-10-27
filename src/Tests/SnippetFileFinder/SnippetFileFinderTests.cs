using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class SnippetFileFinderTests :
    XunitApprovalBase
{
    [Fact]
    public void Nested()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "SnippetFileFinder/Nested");
        var extractor = new SnippetFileFinder();
        var files = extractor.FindFiles(directory);
        ObjectApprover.Verify(files, Scrubber.Scrub);
    }

    [Fact]
    public void Simple()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "SnippetFileFinder/Simple");
        var extractor = new SnippetFileFinder();
        var files = extractor.FindFiles(directory);
        ObjectApprover.Verify(files, Scrubber.Scrub);
    }

    [Fact]
    public void VerifyLambdasAreCalled()
    {
        var directories = new ConcurrentBag<string>();
        var targetDirectory = Path.Combine(AssemblyLocation.CurrentDirectory,
            "SnippetFileFinder/VerifyLambdasAreCalled");
        var extractor = new SnippetFileFinder(
            directoryFilter: path =>
            {
                directories.Add(path);
                return true;
            }
        );
        extractor.FindFiles(targetDirectory);
        ObjectApprover.Verify(directories.OrderBy(file => file), Scrubber.Scrub);
    }

    public SnippetFileFinderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}