using ApprovalTests;
using Xunit;
using Xunit.Abstractions;

public class HeaderWriterTests :
    XunitApprovalBase
{
    [Fact]
    public void DefaultHeader()
    {
        Approvals.Verify(HeaderWriter.DefaultHeader);
    }

    [Fact]
    public void WriteHeaderDefaultHeader()
    {
        Approvals.Verify(HeaderWriter.WriteHeader("thePath", null, "\r\n"));
    }

    [Fact]
    public void WriteHeaderHeaderCustom()
    {
        Approvals.Verify(HeaderWriter.WriteHeader("thePath", @"line1\nline2", "\r\n"));
    }

    public HeaderWriterTests(ITestOutputHelper output) :
        base(output)
    {
    }
}