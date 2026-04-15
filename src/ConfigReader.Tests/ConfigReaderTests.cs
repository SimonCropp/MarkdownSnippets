public class ConfigReaderTests
{
    [Fact]
    public Task Empty()
    {
        var config = ConfigReader.Parse("{}", "filePath");

        return Verify(config);
    }

    [Fact]
    public Task BadJson() =>
        Throws(() => ConfigReader.Parse(
            """
            {
              "ValidateContent": true
              "Convention": "InPlaceOverwrite"
            }
            """,
            "filePath"));

    [Fact]
    public Task Values()
    {
        var stream = File.ReadAllText("allConfig.json");
        var config = ConfigReader.Parse(stream, "filePath");
        return Verify(config);
    }

    [Fact]
    public void FileExcludesToFilter_NullOrEmpty_ReturnsNull()
    {
        Assert.Null(ExcludeToFilterBuilder.FileExcludesToFilter(null));
        Assert.Null(ExcludeToFilterBuilder.FileExcludesToFilter([]));
    }

    [Fact]
    public void FileExcludesToFilter_GlobMatching()
    {
        var filter = ExcludeToFilterBuilder.FileExcludesToFilter(
        [
            "*.verified.txt",
            "*.received.*",
            "ignore?.cs"
        ])!;

        Assert.False(filter("/a/b/Foo.verified.txt"));
        Assert.False(filter(@"C:\x\y\Foo.received.md"));
        Assert.False(filter("ignore1.cs"));
        Assert.True(filter("Foo.cs"));
        Assert.True(filter("Foo.txt"));
        Assert.True(filter("ignoreAB.cs"));
    }

    [Fact]
    public void FileExcludesToFilter_CaseInsensitive()
    {
        var filter = ExcludeToFilterBuilder.FileExcludesToFilter(["*.VERIFIED.txt"])!;
        Assert.False(filter("foo.verified.TXT"));
    }
}