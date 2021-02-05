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
        var directory = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "SnippetFileFinder/Nested"));
        FileFinder finder = new(directory, DocumentConvention.SourceTransform, _ => true);
        var files = finder.FindFiles();
        return Verifier.Verify(files.snippetFiles);
    }

    [Fact]
    public Task Simple()
    {
        var directory = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "SnippetFileFinder/Simple"));
        FileFinder finder = new(directory, DocumentConvention.SourceTransform, _ => true);
        var files = finder.FindFiles();
        return Verifier.Verify(files.snippetFiles);
    }

    [Fact]
    public Task VerifyLambdasAreCalled()
    {
        ConcurrentBag<string> directories = new();
        var directory = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "SnippetFileFinder/VerifyLambdasAreCalled"));
        FileFinder finder = new(directory, DocumentConvention.SourceTransform,
            sharedIncludes: path =>
            {
                directories.Add(path);
                return true;
            }
        );
        var files = finder.FindFiles();
        return Verifier.Verify(directories.OrderBy(file => file));
    }
}