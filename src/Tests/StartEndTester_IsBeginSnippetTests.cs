public class StartEndTester_IsBeginSnippetTests
{
    [Fact]
    public void CanExtractFromXml()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey -->".AsSpan(), "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public Task ShouldThrowForNoKey() =>
        Throws(() => StartEndTester.IsBeginSnippet("<!-- begin-snippet: -->".AsSpan(), "file", out _));

    [Fact]
    public void ShouldNotThrowForNoKeyWithNoSpace() =>
        StartEndTester.IsBeginSnippet("<!--begin-snippet:-->".AsSpan(), "file", out _);

    [Fact]
    public void CanExtractFromXmlWithMissingSpaces()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!--begin-snippet: CodeKey-->".AsSpan(), "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!--  begin-snippet:  CodeKey  -->".AsSpan(), "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey".AsSpan(), "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithUnderScores()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: Code_Key -->".AsSpan(), "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("Code_Key", key);
    }

    [Fact]
    public void CanExtractWithDashes()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: Code-Key -->".AsSpan(), "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("Code-Key", key);
    }

    [Fact]
    public Task ShouldThrowForKeyStartingWithSymbol() =>
        Throws(() =>
            StartEndTester.IsBeginSnippet("<!-- begin-snippet: _key-->".AsSpan(), "file", out _));

    [Fact]
    public Task ShouldThrowForKeyEndingWithSymbol() =>
        Throws(() =>
            StartEndTester.IsBeginSnippet("<!-- begin-snippet: key_ -->".AsSpan(), "file", out _));

    [Fact]
    public void CanExtractWithDifferentEndComments()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("/* begin-snippet: CodeKey */".AsSpan(), "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithDifferentEndCommentsAndNoSpaces()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("/*begin-snippet: CodeKey */".AsSpan(), "file", out var key);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
    }
}