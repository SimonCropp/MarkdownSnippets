using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarkdownSnippets;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class SnippetFileFinderTests
{
    [Fact]
    public Task Nested()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "SnippetFileFinder/Nested");
        SnippetFileFinder finder = new(_=> true);
        var files = finder.FindFiles(directory);
        return Verifier.Verify(files);
    }

    [Fact]
    public Task Simple()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "SnippetFileFinder/Simple");
        SnippetFileFinder finder = new(_=> true);
        var files = finder.FindFiles(directory);
        return Verifier.Verify(files);
    }

    [Fact]
    public Task VerifyLambdasAreCalled()
    {
        ConcurrentBag<string> directories = new();
        var targetDirectory = Path.Combine(AssemblyLocation.CurrentDirectory,
            "SnippetFileFinder/VerifyLambdasAreCalled");
        SnippetFileFinder finder = new(
            shouldIncludeDirectory: path =>
            {
                directories.Add(path);
                return true;
            }
        );
        finder.FindFiles(targetDirectory);
        return Verifier.Verify(directories.OrderBy(file => file));
    }
}