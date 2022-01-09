using MarkdownSnippets;

[UsesVerify]
public class LogBuilderTests
{
    [Fact]
    public Task BuildConfigLogMessage()
    {
        var config = new ConfigResult
        {
            WriteHeader = true,
            Header = @"line1
line2",
            ExcludeDirectories = new List<string> {"Dir1", "Dir2"},
            ExcludeMarkdownDirectories = new List<string> {"Dir3", "Dir4"},
            ExcludeSnippetDirectories = new List<string> {"Dir5", "Dir6"},
            ReadOnly = true,
            LinkFormat = LinkFormat.Tfs,
            UrlsAsSnippets = new List<string> {"Url1", "Url2"},
            TocLevel = 5,
            MaxWidth = 80,
            Convention = DocumentConvention.InPlaceOverwrite,
        };
        var message = LogBuilder.BuildConfigLogMessage("theRoot", config, "theConfigFilePath");
        return Verify(message);
    }

    [Fact]
    public Task BuildConfigLogMessageMinimal()
    {
        var config = new ConfigResult();
        var message = LogBuilder.BuildConfigLogMessage("theRoot", config, "theConfigFilePath");
        return Verify(message);
    }
}