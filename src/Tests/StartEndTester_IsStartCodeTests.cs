using ApprovalTests;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class StartEndTester_IsStartCodeTests : 
    TestBase
{
    [Fact]
    public void CanExtractFromXml()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey -->", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void ShouldThrowForNoKey()
    {
        var exception = Assert.Throws<SnippetReadingException>(() => StartEndTester.IsStartCode("<!-- startcode -->", "file", out _));
        Assert.Equal("No Key could be derived. Line: '<!-- startcode -->'.", exception.Message);
    }

    [Fact]
    public void ShouldNotThrowForNoKeyWithNoSpace()
    {
        StartEndTester.IsStartCode("<!--startcode-->", "file", out _);
    }

    [Fact]
    public void CanExtractFromXmlWithMissingSpaces()
    {
        var isStartCode = StartEndTester.IsStartCode("<!--startcode CodeKey-->", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        var isStartCode = StartEndTester.IsStartCode("<!--  startcode  CodeKey  -->", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode CodeKey", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithUnderScores()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code_Key -->", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("Code_Key", key);
    }

    [Fact]
    public void CanExtractWithDashes()
    {
        var isStartCode = StartEndTester.IsStartCode("<!-- startcode Code-Key -->", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("Code-Key", key);
    }

    [Fact]
    public void ShouldThrowForKeyStartingWithSymbol()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsStartCode("<!-- startcode _key-->", "file", out _));

        Approvals.Verify(exception.Message);
    }

    [Fact]
    public void ShouldThrowForKeyEndingWithSymbol()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsStartCode("<!-- startcode key_ -->", "file", out _));
        Approvals.Verify(exception.Message);
    }

    [Fact]
    public void CanExtractWithDifferentEndComments()
    {
        var isStartCode = StartEndTester.IsStartCode("/* startcode CodeKey */", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithDifferentEndCommentsAndNoSpaces()
    {
        var isStartCode = StartEndTester.IsStartCode("/*startcode CodeKey */", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    public StartEndTester_IsStartCodeTests(ITestOutputHelper output) :
        base(output)
    {
    }
}