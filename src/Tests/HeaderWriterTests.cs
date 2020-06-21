using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

[UsesVerify]
public class HeaderWriterTests
{
    [Fact]
    public Task DefaultHeader()
    {
        return Verifier.Verify(HeaderWriter.DefaultHeader);
    }

    [Fact]
    public Task WriteHeaderDefaultHeader()
    {
        return Verifier.Verify(HeaderWriter.WriteHeader("thePath", null, "\r\n"));
    }

    [Fact]
    public Task WriteHeaderHeaderCustom()
    {
        return Verifier.Verify(HeaderWriter.WriteHeader("thePath", @"line1\nline2", "\r\n"));
    }
}