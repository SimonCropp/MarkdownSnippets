using System.Threading.Tasks;
using VerifyXunit;
using MarkdownSnippets;
using Xunit;

[UsesVerify]
public class LogBuilderTests
{
    [Fact]
    public Task BuildConfigLogMessage()
    {
        ConfigResult config = new()
        {
            WriteHeader = true,
            Header = @"line1
line2",
            Exclude = new(){"Dir1", "Dir2"},
            ReadOnly = true,
            LinkFormat = LinkFormat.Tfs,
            UrlsAsSnippets = new(){"Url1", "Url2"},
            TocLevel = 5,
            MaxWidth = 80,
            Convention = DocumentConvention.InPlaceOverwrite,
        };
        var message = LogBuilder.BuildConfigLogMessage("theRoot", config, "theConfigFilePath");
        return Verifier.Verify(message);
    }

    [Fact]
    public Task BuildConfigLogMessageMinimal()
    {
        ConfigResult config = new();
        var message = LogBuilder.BuildConfigLogMessage("theRoot", config, "theConfigFilePath");
        return Verifier.Verify(message);
    }
}