using MarkdownSnippets;
using Xunit;

public class StartEndTester_IsStartRegionTests : TestBase
{
    [Fact]
    public void CanExtractFromXml()
    {
        StartEndTester.IsStartRegion("#region CodeKey", out var key);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void ShouldThrowForKeyStartingWithSymbol()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsStartRegion("#region _key", out _));
        Assert.Equal("Key should not start or end with symbols. Key: _key", exception.Message);
    }

    [Fact]
    public void ShouldThrowForKeyEndingWithSymbol()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsStartRegion("#region key_ ", out _));
        Assert.Equal("Key should not start or end with symbols. Key: key_", exception.Message);
    }


    [Fact]
    public void ShouldIgnoreForNoKey()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            StartEndTester.IsStartRegion("#region ", out _));
        Assert.Equal("No Key could be derived. Line: '#region '.", exception.Message);
    }

    [Fact]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        StartEndTester.IsStartRegion("#region  CodeKey   ", out var key);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        StartEndTester.IsStartRegion("#region CodeKey", out var key);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithUnderScores()
    {
        StartEndTester.IsStartRegion("#region Code_Key", out var key);
        Assert.Equal("Code_Key", key);
    }

    [Fact]
    public void CanExtractWithDashes()
    {
        StartEndTester.IsStartRegion("#region Code-Key", out var key);
        Assert.Equal("Code-Key", key);
    }
}