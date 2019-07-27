using ApprovalTests;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class StartEndTester_IsStartRegionTests :
    TestBase
{
    [Fact]
    public void CanExtractFromXml()
    {
        StartEndTester.IsStartRegion("#region CodeKey", "file", out var key);
        Assert.Equal("codekey", key);
    }

    [Fact]
    public void ShouldThrowForKeyStartingWithSymbol()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsStartRegion("#region _key", "file", out _));
        Approvals.Verify(exception.Message);
    }

    [Fact]
    public void ShouldThrowForKeyEndingWithSymbol()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsStartRegion("#region key_ ", "file", out _));
        Approvals.Verify(exception.Message);
    }


    [Fact]
    public void ShouldIgnoreForNoKey()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsStartRegion("#region ", "file", out _));
        Assert.Equal("No Key could be derived. Line: '#region '.", exception.Message);
    }

    [Fact]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        StartEndTester.IsStartRegion("#region  CodeKey   ", "file", out var key);
        Assert.Equal("codekey", key);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        StartEndTester.IsStartRegion("#region CodeKey", "file", out var key);
        Assert.Equal("codekey", key);
    }

    [Fact]
    public void CanExtractWithUnderScores()
    {
        StartEndTester.IsStartRegion("#region Code_Key", "file", out var key);
        Assert.Equal("code_key", key);
    }

    [Fact]
    public void CanExtractWithDashes()
    {
        StartEndTester.IsStartRegion("#region Code-Key", "file", out var key);
        Assert.Equal("code-key", key);
    }

    public StartEndTester_IsStartRegionTests(ITestOutputHelper output) :
        base(output)
    {
    }
}