using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

public class IncludeFileFinderTests :
    XunitApprovalBase
{
    [Fact]
    public void Nested()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "IncludeFileFinder/Nested");
        var finder = new IncludeFileFinder();
        var files = finder.FindFiles(directory);
        ObjectApprover.Verify(files, Scrubber.Scrub);
    }

    [Fact]
    public void Simple()
    {
        var directory = Path.Combine(AssemblyLocation.CurrentDirectory, "IncludeFileFinder/Simple");
        var finder = new IncludeFileFinder();
        var files = finder.FindFiles(directory);
        ObjectApprover.Verify(files, Scrubber.Scrub);
    }

    [Fact]
    public void VerifyLambdasAreCalled()
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
        ObjectApprover.Verify(directories.OrderBy(file => file), Scrubber.Scrub);
    }

    public IncludeFileFinderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}