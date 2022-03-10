public class StartEndTester_IsStartRegionTests
{
    [Fact]
    public void CanExtractFromXml()
    {
        StartEndTester.IsStartRegion("#region CodeKey", out var key);
        Assert.Equal("codekey", key);
    }

    [Fact]
    public void ShouldThrowForKeyStartingWithSymbol() =>
        Assert.False(StartEndTester.IsStartRegion("#region _key", out _));

    [Fact]
    public void WithSpaces() =>
        Assert.False(StartEndTester.IsStartRegion("#region the text", out _));

    [Fact]
    public void ShouldThrowForKeyEndingWithSymbol() =>
        Assert.False(StartEndTester.IsStartRegion("#region key_ ", out _));

    [Fact]
    public void ShouldIgnoreForNoKey() =>
        Assert.False(StartEndTester.IsStartRegion("#region ", out _));

    [Fact]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        StartEndTester.IsStartRegion("#region  CodeKey   ", out var key);
        Assert.Equal("codekey", key);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        StartEndTester.IsStartRegion("#region CodeKey", out var key);
        Assert.Equal("codekey", key);
    }

    [Fact]
    public void CanExtractWithUnderScores()
    {
        StartEndTester.IsStartRegion("#region Code_Key", out var key);
        Assert.Equal("code_key", key);
    }

    [Fact]
    public void CanExtractWithDashes()
    {
        StartEndTester.IsStartRegion("#region Code-Key", out var key);
        Assert.Equal("code-key", key);
    }
}