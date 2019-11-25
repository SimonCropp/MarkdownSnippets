using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarkdownSnippets;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class SnippetFileFinderTests :
    VerifyBase
{
    [Fact]
    public Task Nested()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "SnippetFileFinder/Nested");
        var finder = new SnippetFileFinder();
        var files = finder.FindFiles(directory);
        return Verify(files);
    }

    [Fact]
    public Task Simple()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "SnippetFileFinder/Simple");
        var finder = new SnippetFileFinder();
        var files = finder.FindFiles(directory);
        return Verify(files);
    }

    [Fact]
    public Task VerifyLambdasAreCalled()
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
        return Verify(directories.OrderBy(file => file));
    }

    public SnippetFileFinderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}