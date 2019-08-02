using ApprovalTests;
using Xunit;
using Xunit.Abstractions;

public class HeaderWriterTests :
    TestBase
{
    [Fact]
    public void DefaultHeader()
    {
        Approvals.Verify(HeaderWriter.DefaultHeader);
    }

    public HeaderWriterTests(ITestOutputHelper output) :
        base(output)
    {
    }
}