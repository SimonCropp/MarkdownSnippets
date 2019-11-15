using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class IncludeFinderTests :
    XunitApprovalBase
{
    [Fact]
    public void Simple()
    {
        var finder = new IncludeFinder();
        var includes = finder.ReadIncludes("IncludeFinder");
        ObjectApprover.Verify(includes, Scrubber.Scrub);
    }

    public IncludeFinderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}