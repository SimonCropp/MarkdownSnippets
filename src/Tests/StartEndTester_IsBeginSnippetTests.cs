using VerifyXunit;
using Xunit;

[UsesVerify]
public class StartEndTester_IsBeginSnippetTests
{
    [Fact]
    public void CanExtractFromXml()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey -->", "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("codekey", key);
    }

    [Fact]
    public Task ShouldThrowForNoKey()
    {
        return Verifier.Throws(() => StartEndTester.IsBeginSnippet("<!-- begin-snippet: -->", "file", out _));
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
    public Task ShouldThrowForKeyStartingWithSymbol()
    {
        return Verifier.Throws(() =>
            StartEndTester.IsBeginSnippet("<!-- begin-snippet: _key-->", "file", out _));
    }

    [Fact]
    public Task ShouldThrowForKeyEndingWithSymbol()
    {
        return Verifier.Throws(() =>
            StartEndTester.IsBeginSnippet("<!-- begin-snippet: key_ -->", "file", out _));
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
}