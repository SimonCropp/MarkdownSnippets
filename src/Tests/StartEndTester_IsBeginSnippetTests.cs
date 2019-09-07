using ApprovalTests;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class StartEndTester_IsBeginSnippetTests :
    XunitApprovalBase
{
    [Fact]
    public void CanExtractFromXml()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey -->", "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("codekey", key);
    }

    [Fact]
    public void ShouldThrowForNoKey()
    {
        var exception = Assert.Throws<SnippetReadingException>(() => StartEndTester.IsBeginSnippet("<!-- begin-snippet: -->", "file", out _));
        Approvals.Verify(exception.Message);
    }

    [Fact]
    public void ShouldNotThrowForNoKeyWithNoSpace()
    {
        StartEndTester.IsBeginSnippet("<!--begin-snippet:-->", "file", out _);
    }

    [Fact]
    public void CanExtractFromXmlWithMissingSpaces()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!--begin-snippet: CodeKey-->", "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("codekey", key);
    }

    [Fact]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!--  begin-snippet:  CodeKey  -->", "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("codekey", key);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey", "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("codekey", key);
    }

    [Fact]
    public void CanExtractWithUnderScores()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: Code_Key -->", "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("code_key", key);
    }

    [Fact]
    public void CanExtractWithDashes()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: Code-Key -->", "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("code-key", key);
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
        var isBeginSnippet = StartEndTester.IsBeginSnippet("/* begin-snippet: CodeKey */", "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("codekey", key);
    }

    [Fact]
    public void CanExtractWithDifferentEndCommentsAndNoSpaces()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("/*begin-snippet: CodeKey */", "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("codekey", key);
    }

    public StartEndTester_IsBeginSnippetTests(ITestOutputHelper output) :
        base(output)
    {
    }
}