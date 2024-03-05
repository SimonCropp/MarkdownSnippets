public class StartEndTester_IsStartRegionTests
{
    [Fact]
    public void CanExtractFromXml()
    {
        StartEndTester.IsStartRegion("#region CodeKey".AsSpan(), out var key);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void ShouldThrowForKeyStartingWithSymbol() =>
        Assert.False(StartEndTester.IsStartRegion("#region _key".AsSpan(), out _));

    [Fact]
    public void WithSpaces() =>
        Assert.False(StartEndTester.IsStartRegion("#region the text".AsSpan(), out _));

    [Fact]
    public void ShouldThrowForKeyEndingWithSymbol() =>
        Assert.False(StartEndTester.IsStartRegion("#region key_ ".AsSpan(), out _));

    [Fact]
    public void ShouldIgnoreForNoKey() =>
        Assert.False(StartEndTester.IsStartRegion("#region ".AsSpan(), out _));

    [Fact]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        StartEndTester.IsStartRegion("#region  CodeKey   ".AsSpan(), out var key);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        StartEndTester.IsStartRegion("#region CodeKey".AsSpan(), out var key);
        Assert.Equal("CodeKey", key);
    }

    [Fact]
    public void CanExtractWithUnderScores()
    {
        StartEndTester.IsStartRegion("#region Code_Key".AsSpan(), out var key);
        Assert.Equal("Code_Key", key);
    }

    [Fact]
    public void CanExtractWithDashes()
    {
        StartEndTester.IsStartRegion("#region Code-Key".AsSpan(), out var key);
        Assert.Equal("Code-Key", key);
    }
}