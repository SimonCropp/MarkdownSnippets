using ApprovalTests;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class StartEndTesterIsBeginSnippetTests :
    TestBase
{
    [Fact]
    public void CanExtractFromXml()
    {
        var isStartCode = StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey -->", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void ShouldThrowForNoKey()
    {
        var exception = Assert.Throws<SnippetReadingException>(() => StartEndTester.IsBeginSnippet("<!-- begin-snippet: -->", "file", out _));
        Assert.Equal("No Key could be derived. Line: '<!-- begin-snippet: -->'.", exception.Message);
    }

    [Fact]
    public void ShouldNotThrowForNoKeyWithNoSpace()
    {
        StartEndTester.IsBeginSnippet("<!--begin-snippet:-->", "file", out _);
    }

    [Fact]
    public void CanExtractFromXmlWithMissingSpaces()
    {
        var isStartCode = StartEndTester.IsBeginSnippet("<!--begin-snippet: CodeKey-->", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        var isStartCode = StartEndTester.IsBeginSnippet("<!--  begin-snippet:  CodeKey  -->", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        var isStartCode = StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithUnderScores()
    {
        var isStartCode = StartEndTester.IsBeginSnippet("<!-- begin-snippet: Code_Key -->", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("Code_Key", key);
    }

    [Fact]
    public void CanExtractWithDashes()
    {
        var isStartCode = StartEndTester.IsBeginSnippet("<!-- begin-snippet: Code-Key -->", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("Code-Key", key);
    }

    [Fact]
    public void ShouldThrowForKeyStartingWithSymbol()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsBeginSnippet("<!-- begin-snippet: _key-->", "file", out _));

        Approvals.Verify(exception.Message);
    }

    [Fact]
    public void ShouldThrowForKeyEndingWithSymbol()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsBeginSnippet("<!-- begin-snippet: key_ -->", "file", out _));
        Approvals.Verify(exception.Message);
    }

    [Fact]
    public void CanExtractWithDifferentEndComments()
    {
        var isStartCode = StartEndTester.IsBeginSnippet("/* begin-snippet: CodeKey */", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithDifferentEndCommentsAndNoSpaces()
    {
        var isStartCode = StartEndTester.IsBeginSnippet("/*begin-snippet: CodeKey */", "file", out var key);
        Assert.True(isStartCode);
        Assert.Equal("CodeKey", key);
    }

    public StartEndTesterIsBeginSnippetTests(ITestOutputHelper output) :
        base(output)
    {
    }
}