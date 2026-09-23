public class ConfigReaderTests
{
    [Test]
    public Task Empty()
    {
        var config = ConfigReader.Parse("{}", "filePath");

        return Verify(config);
    }

    [Test]
    public Task BadJson() =>
        Throws(() => ConfigReader.Parse(
            """
            {
              "ValidateContent": true
              "Convention": "InPlaceOverwrite"
            }
            """,
            "filePath"));

    [Test]
    public Task Values()
    {
        var stream = File.ReadAllText("allConfig.json");
        var config = ConfigReader.Parse(stream, "filePath");
        return Verify(config);
    }

    [Test]
    public async Task FileExcludesToFilter_NullOrEmpty_ReturnsNull()
    {
        await Assert.That(ExcludeToFilterBuilder.FileExcludesToFilter(null)).IsNull();
        await Assert.That(ExcludeToFilterBuilder.FileExcludesToFilter([])).IsNull();
    }

    [Test]
    public async Task FileExcludesToFilter_GlobMatching()
    {
        var filter = ExcludeToFilterBuilder.FileExcludesToFilter(
        [
            "*.verified.txt",
            "*.received.*",
            "ignore?.cs"
        ])!;

        await Assert.That(filter("/a/b/Foo.verified.txt")).IsFalse();
        await Assert.That(filter(@"C:\x\y\Foo.received.md")).IsFalse();
        await Assert.That(filter("ignore1.cs")).IsFalse();
        await Assert.That(filter("Foo.cs")).IsTrue();
        await Assert.That(filter("Foo.txt")).IsTrue();
        await Assert.That(filter("ignoreAB.cs")).IsTrue();
    }

    [Test]
    public async Task FileExcludesToFilter_CaseInsensitive()
    {
        var filter = ExcludeToFilterBuilder.FileExcludesToFilter(["*.VERIFIED.txt"])!;
        await Assert.That(filter("foo.verified.TXT")).IsFalse();
    }
}