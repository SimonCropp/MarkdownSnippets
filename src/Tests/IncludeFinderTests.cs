using System.Threading.Tasks;
using MarkdownSnippets;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class IncludeFinderTests :
    VerifyBase
{
    [Fact]
    public Task Simple()
    {
        var finder = new IncludeFinder();
        var includes = finder.ReadIncludes("IncludeFinder");
        return Verify(includes);
    }

    public IncludeFinderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}