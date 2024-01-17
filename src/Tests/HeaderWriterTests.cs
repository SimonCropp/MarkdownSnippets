public class HeaderWriterTests
{
    [Fact]
    public Task DefaultHeader() =>
        Verify(HeaderWriter.DefaultHeader);

    [Fact]
    public Task WriteHeaderDefaultHeader() =>
        Verify(HeaderWriter.WriteHeader("thePath", null, "\r\n"));

    [Fact]
    public Task WriteHeaderHeaderCustom() =>
        Verify(HeaderWriter.WriteHeader("thePath", @"line1\nline2", "\r\n"));
}