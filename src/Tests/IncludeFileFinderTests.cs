using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class IncludeFileFinderTests :
    VerifyBase
{
    [Fact]
    public Task Nested()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "IncludeFileFinder/Nested");
        var finder = new IncludeFileFinder();
        var files = finder.FindFiles(directory);
        return Verify(files);
    }

    [Fact]
    public Task Simple()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "IncludeFileFinder/Simple");
        var finder = new IncludeFileFinder();
        var files = finder.FindFiles(directory);
        return Verify(files);
    }

    [Fact]
    public Task VerifyLambdasAreCalled()
    {
        var directories = new ConcurrentBag<string>();
        var targetDirectory = Path.Combine(AssemblyLocation.CurrentDirectory,
            "IncludeFileFinder/VerifyLambdasAreCalled");
        var finder = new IncludeFileFinder(
            directoryFilter: path =>
            {
                directories.Add(path);
                return true;
            }
        );
        finder.FindFiles(targetDirectory);
        return Verify(directories.OrderBy(file => file));
    }

    public IncludeFileFinderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}