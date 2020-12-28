using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class IncludeFileFinderTests
{
    [Fact]
    public Task Nested()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "IncludeFileFinder/Nested");
        IncludeFileFinder finder = new();
        var files = finder.FindFiles(directory);
        return Verifier.Verify(files);
    }

    [Fact]
    public Task Simple()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "IncludeFileFinder/Simple");
        IncludeFileFinder finder = new();
        var files = finder.FindFiles(directory);
        return Verifier.Verify(files);
    }

    [Fact]
    public Task VerifyLambdasAreCalled()
    {
        ConcurrentBag<string> directories = new();
        var targetDirectory = Path.Combine(AssemblyLocation.CurrentDirectory,
            "IncludeFileFinder/VerifyLambdasAreCalled");
        IncludeFileFinder finder = new(
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