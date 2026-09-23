public class StartEndTester_IsStartRegionTests
{
    [Test]
    public async Task CanExtractFromXml()
    {
        StartEndTester.IsStartRegion("#region CodeKey", out var keySpan);
        var key = keySpan.ToString();
        await Assert.That(key).IsEqualTo("CodeKey");
    }

    [Test]
    public async Task ShouldThrowForKeyStartingWithSymbol() =>
        await Assert.That(StartEndTester.IsStartRegion("#region _key", out _)).IsFalse();

    [Test]
    public async Task WithSpaces() =>
        await Assert.That(StartEndTester.IsStartRegion("#region the text", out _)).IsFalse();

    [Test]
    public async Task ShouldThrowForKeyEndingWithSymbol() =>
        await Assert.That(StartEndTester.IsStartRegion("#region key_ ", out _)).IsFalse();

    [Test]
    public async Task ShouldIgnoreForNoKey() =>
        await Assert.That(StartEndTester.IsStartRegion("#region ", out _)).IsFalse();

    [Test]
    public async Task CanExtractFromXmlWithExtraSpaces()
    {
        StartEndTester.IsStartRegion("#region  CodeKey   ", out var keySpan);
        var key = keySpan.ToString();
        await Assert.That(key).IsEqualTo("CodeKey");
    }

    [Test]
    public async Task CanExtractWithNoTrailingCharacters()
    {
        StartEndTester.IsStartRegion("#region CodeKey", out var keySpan);
        var key = keySpan.ToString();
        await Assert.That(key).IsEqualTo("CodeKey");
    }

    [Test]
    public async Task CanExtractWithUnderScores()
    {
        StartEndTester.IsStartRegion("#region Code_Key", out var keySpan);
        var key = keySpan.ToString();
        await Assert.That(key).IsEqualTo("Code_Key");
    }

    [Test]
    public async Task CanExtractWithDashes()
    {
        StartEndTester.IsStartRegion("#region Code-Key", out var keySpan);
        var key = keySpan.ToString();
        await Assert.That(key).IsEqualTo("Code-Key");
    }
}