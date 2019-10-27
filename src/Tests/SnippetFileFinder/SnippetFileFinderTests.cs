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
        var finder = new SnippetFileFinder();
        var files = finder.FindFiles(directory);
        ObjectApprover.Verify(files, Scrubber.Scrub);
    }

    [Fact]
    public void Simple()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "SnippetFileFinder/Simple");
        var finder = new SnippetFileFinder();
        var files = finder.FindFiles(directory);
        ObjectApprover.Verify(files, Scrubber.Scrub);
    }

    [Fact]
    public void VerifyLambdasAreCalled()
    {
        var directories = new ConcurrentBag<string>();
        var targetDirectory = Path.Combine(AssemblyLocation.CurrentDirectory,
            "SnippetFileFinder/VerifyLambdasAreCalled");
        var finder = new SnippetFileFinder(
            directoryFilter: path =>
            {
                directories.Add(path);
                return true;
            }
        );
        finder.FindFiles(targetDirectory);
        ObjectApprover.Verify(directories.OrderBy(file => file), Scrubber.Scrub);
    }

    public SnippetFileFinderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}