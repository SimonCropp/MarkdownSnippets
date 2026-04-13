public class StartEndTester_IsBeginSnippetTests
{
    [Fact]
    public void CanExtractFromXml()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey -->", "file", out var key, out _);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public Task ShouldThrowForNoKey() =>
        Throws(() => StartEndTester.IsBeginSnippet("<!-- begin-snippet: -->", "file", out _, out _));

    [Fact]
    public void ShouldNotThrowForNoKeyWithNoSpace() =>
        StartEndTester.IsBeginSnippet("<!--begin-snippet:-->", "file", out _, out _);

    [Fact]
    public void CanExtractFromXmlWithMissingSpaces()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!--begin-snippet: CodeKey-->", "file", out var key, out _);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!--  begin-snippet:  CodeKey  -->", "file", out var key, out _);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey", "file", out var key, out _);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithUnderScores()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: Code_Key -->", "file", out var key, out _);
        Assert.True(isBeginSnippet);
        Assert.Equal("Code_Key", key);
    }

    [Fact]
    public void CanExtractWithDashes()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: Code-Key -->", "file", out var key, out _);
        Assert.True(isBeginSnippet);
        Assert.Equal("Code-Key", key);
    }

    [Fact]
    public Task ShouldThrowForKeyStartingWithSymbol() =>
        Throws(() =>
            StartEndTester.IsBeginSnippet("<!-- begin-snippet: _key-->", "file", out _, out _));

    [Fact]
    public Task ShouldThrowForKeyEndingWithSymbol() =>
        Throws(() =>
            StartEndTester.IsBeginSnippet("<!-- begin-snippet: key_ -->", "file", out _, out _));

    [Fact]
    public void CanExtractWithDifferentEndComments()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("/* begin-snippet: CodeKey */", "file", out var key,out _);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithDifferentEndCommentsAndNoSpaces()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("/*begin-snippet: CodeKey */", "file", out var key, out _);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithExpressiveCodeWithHtmlSnippet()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("""<!--begin-snippet: CodeKey(title="Program.cs" {1-3})-->""", "file", out var key, out var block);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
        Assert.Equal("""title="Program.cs" {1-3}""", block);
    }

    [Fact]
    public void CanExtractWithExpressiveCodeWithCsharpComment()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("""/*begin-snippet: CodeKey(title="Program.cs" {1-3})*/""", "file", out var key, out var expressive);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
        Assert.Equal("""title="Program.cs" {1-3}""", expressive);
    }
    [Fact]
    public void CanExtractWithExpressiveCodeWithHtmlSnippetTrailingWhitespace()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("""<!--begin-snippet: CodeKey(title="Program.cs" {1-3})  -->""", "file", out var key, out var block);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
        Assert.Equal("""title="Program.cs" {1-3}""", block);
    }

    [Fact]
    public void CanExtractWithExpressiveCodeWithCsharpCommentTrailingWhitespace()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("""/*begin-snippet: CodeKey(title="Program.cs" {1-3})  */""", "file", out var key, out var expressive);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
        Assert.Equal("""title="Program.cs" {1-3}""", expressive);
    }

    [Fact]
    public void CanExtractLanguageOverride()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey (lang=json) -->", "file", out var key, out var expressive, out var language);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
        Assert.Equal("json", language.ToString());
        Assert.Equal(0, expressive.Length);
    }

    [Fact]
    public void CanExtractLanguageOverrideWithExpressiveCode()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("""<!-- begin-snippet: CodeKey (lang=json title="a.json") -->""", "file", out var key, out var expressive, out var language);
        Assert.True(isBeginSnippet);
        Assert.Equal("CodeKey", key);
        Assert.Equal("json", language.ToString());
        Assert.Equal("""title="a.json" """.TrimEnd(), expressive.ToString());
    }

    [Fact]
    public Task ShouldThrowForInvalidLanguageValue() =>
        Throws(() =>
            StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey (lang=C#) -->", "file", out _, out _, out _));

    [Fact]
    public Task ShouldThrowForEmptyLanguageValue() =>
        Throws(() =>
            StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey (lang=) -->", "file", out _, out _, out _));
}