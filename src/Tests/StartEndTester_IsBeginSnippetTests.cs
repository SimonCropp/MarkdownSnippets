public class StartEndTester_IsBeginSnippetTests
{
    [Test]
    public async Task CanExtractFromXml()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey -->", "file", out var keySpan, out _);
        var key = keySpan.ToString();
        await Assert.That(isBeginSnippet).IsTrue();
        await Assert.That(key).IsEqualTo("CodeKey");
    }

    [Test]
    public Task ShouldThrowForNoKey() =>
        Throws(() => StartEndTester.IsBeginSnippet("<!-- begin-snippet: -->", "file", out _, out _))
            .Snapshot(
                """
                {
                  Type: SnippetReadingException,
                  Message:
                No Key could be derived.
                Path: file
                Line: '<!-- begin-snippet: -->'
                }
                """);

    [Test]
    public void ShouldNotThrowForNoKeyWithNoSpace() =>
        StartEndTester.IsBeginSnippet("<!--begin-snippet:-->", "file", out _, out _);

    [Test]
    public async Task CanExtractFromXmlWithMissingSpaces()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!--begin-snippet: CodeKey-->", "file", out var keySpan, out _);
        var key = keySpan.ToString();
        await Assert.That(isBeginSnippet).IsTrue();
        await Assert.That(key).IsEqualTo("CodeKey");
    }

    [Test]
    public async Task CanExtractFromXmlWithExtraSpaces()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!--  begin-snippet:  CodeKey  -->", "file", out var keySpan, out _);
        var key = keySpan.ToString();
        await Assert.That(isBeginSnippet).IsTrue();
        await Assert.That(key).IsEqualTo("CodeKey");
    }

    [Test]
    public async Task CanExtractWithNoTrailingCharacters()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey", "file", out var keySpan, out _);
        var key = keySpan.ToString();
        await Assert.That(isBeginSnippet).IsTrue();
        await Assert.That(key).IsEqualTo("CodeKey");
    }

    [Test]
    public async Task CanExtractWithUnderScores()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: Code_Key -->", "file", out var keySpan, out _);
        var key = keySpan.ToString();
        await Assert.That(isBeginSnippet).IsTrue();
        await Assert.That(key).IsEqualTo("Code_Key");
    }

    [Test]
    public async Task CanExtractWithDashes()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: Code-Key -->", "file", out var keySpan, out _);
        var key = keySpan.ToString();
        await Assert.That(isBeginSnippet).IsTrue();
        await Assert.That(key).IsEqualTo("Code-Key");
    }

    [Test]
    public Task ShouldThrowForKeyStartingWithSymbol() =>
        Throws(() =>
            StartEndTester.IsBeginSnippet("<!-- begin-snippet: _key-->", "file", out _, out _))
            .Snapshot(
                """
                {
                  Type: SnippetReadingException,
                  Message:
                Key cannot contain whitespace or start/end with symbols.
                Key: _key
                Path: file
                Line: <!-- begin-snippet: _key-->
                }
                """);

    [Test]
    public Task ShouldThrowForKeyEndingWithSymbol() =>
        Throws(() =>
            StartEndTester.IsBeginSnippet("<!-- begin-snippet: key_ -->", "file", out _, out _))
            .Snapshot(
                """
                {
                  Type: SnippetReadingException,
                  Message:
                Key cannot contain whitespace or start/end with symbols.
                Key: key_
                Path: file
                Line: <!-- begin-snippet: key_ -->
                }
                """);

    [Test]
    public async Task CanExtractWithDifferentEndComments()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("/* begin-snippet: CodeKey */", "file", out var keySpan,out _);
        var key = keySpan.ToString();
        await Assert.That(isBeginSnippet).IsTrue();
        await Assert.That(key).IsEqualTo("CodeKey");
    }

    [Test]
    public async Task CanExtractWithDifferentEndCommentsAndNoSpaces()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("/*begin-snippet: CodeKey */", "file", out var keySpan, out _);
        var key = keySpan.ToString();
        await Assert.That(isBeginSnippet).IsTrue();
        await Assert.That(key).IsEqualTo("CodeKey");
    }

    [Test]
    public async Task CanExtractWithExpressiveCodeWithHtmlSnippet()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("""<!--begin-snippet: CodeKey(title="Program.cs" {1-3})-->""", "file", out var keySpan, out var blockSpan);
        var key = keySpan.ToString();
        var block = blockSpan.ToString();
        await Assert.That(isBeginSnippet).IsTrue();
        await Assert.That(key).IsEqualTo("CodeKey");
        await Assert.That(block).IsEqualTo("""title="Program.cs" {1-3}""");
    }

    [Test]
    public async Task CanExtractWithExpressiveCodeWithCsharpComment()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("""/*begin-snippet: CodeKey(title="Program.cs" {1-3})*/""", "file", out var keySpan, out var expressiveSpan);
        var key = keySpan.ToString();
        var expressive = expressiveSpan.ToString();
        await Assert.That(isBeginSnippet).IsTrue();
        await Assert.That(key).IsEqualTo("CodeKey");
        await Assert.That(expressive).IsEqualTo("""title="Program.cs" {1-3}""");
    }
    [Test]
    public async Task CanExtractWithExpressiveCodeWithHtmlSnippetTrailingWhitespace()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("""<!--begin-snippet: CodeKey(title="Program.cs" {1-3})  -->""", "file", out var keySpan, out var blockSpan);
        var key = keySpan.ToString();
        var block = blockSpan.ToString();
        await Assert.That(isBeginSnippet).IsTrue();
        await Assert.That(key).IsEqualTo("CodeKey");
        await Assert.That(block).IsEqualTo("""title="Program.cs" {1-3}""");
    }

    [Test]
    public async Task CanExtractWithExpressiveCodeWithCsharpCommentTrailingWhitespace()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("""/*begin-snippet: CodeKey(title="Program.cs" {1-3})  */""", "file", out var keySpan, out var expressiveSpan);
        var key = keySpan.ToString();
        var expressive = expressiveSpan.ToString();
        await Assert.That(isBeginSnippet).IsTrue();
        await Assert.That(key).IsEqualTo("CodeKey");
        await Assert.That(expressive).IsEqualTo("""title="Program.cs" {1-3}""");
    }

    [Test]
    public async Task CanExtractLanguageOverride()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey (lang=json) -->", "file", out var keySpan, out var expressiveSpan, out var languageSpan);
        var key = keySpan.ToString();
        var expressive = expressiveSpan.ToString();
        var language = languageSpan.ToString();
        await Assert.That(isBeginSnippet).IsTrue();
        await Assert.That(key).IsEqualTo("CodeKey");
        await Assert.That(language).IsEqualTo("json");
        await Assert.That(expressive.Length).IsEqualTo(0);
    }

    [Test]
    public async Task CanExtractLanguageOverrideWithExpressiveCode()
    {
        var isBeginSnippet = StartEndTester.IsBeginSnippet("""<!-- begin-snippet: CodeKey (lang=json title="a.json") -->""", "file", out var keySpan, out var expressiveSpan, out var languageSpan);
        var key = keySpan.ToString();
        var expressive = expressiveSpan.ToString();
        var language = languageSpan.ToString();
        await Assert.That(isBeginSnippet).IsTrue();
        await Assert.That(key).IsEqualTo("CodeKey");
        await Assert.That(language).IsEqualTo("json");
        await Assert.That(expressive).IsEqualTo("""title="a.json" """.TrimEnd());
    }

    [Test]
    public Task ShouldThrowForInvalidLanguageValue() =>
        Throws(() =>
            StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey (lang=C#) -->", "file", out _, out _, out _))
            .Snapshot(
                """
                {
                  Type: SnippetReadingException,
                  Message:
                lang value must be lowercase alphanumeric.
                Key: CodeKey
                Value: C#
                Path: file
                Line: <!-- begin-snippet: CodeKey (lang=C#) -->
                }
                """);

    [Test]
    public Task ShouldThrowForEmptyLanguageValue() =>
        Throws(() =>
            StartEndTester.IsBeginSnippet("<!-- begin-snippet: CodeKey (lang=) -->", "file", out _, out _, out _))
            .Snapshot(
                """
                {
                  Type: SnippetReadingException,
                  Message:
                lang= must have a value.
                Key: CodeKey
                Path: file
                Line: <!-- begin-snippet: CodeKey (lang=) -->
                }
                """);
}