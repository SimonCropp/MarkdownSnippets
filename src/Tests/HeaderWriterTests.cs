using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class HeaderWriterTests :
    VerifyBase
{
    [Fact]
    public Task DefaultHeader()
    {
        return Verify(HeaderWriter.DefaultHeader);
    }

    [Fact]
    public Task WriteHeaderDefaultHeader()
    {
        return Verify(HeaderWriter.WriteHeader("thePath", null, "\r\n"));
    }

    [Fact]
    public Task WriteHeaderHeaderCustom()
    {
        return Verify(HeaderWriter.WriteHeader("thePath", @"line1\nline2", "\r\n"));
    }

    public HeaderWriterTests(ITestOutputHelper output) :
        base(output)
    {
    }
}