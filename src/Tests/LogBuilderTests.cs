using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyXunit;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class LogBuilderTests :
    VerifyBase
{
    [Fact]
    public Task BuildConfigLogMessage()
    {
        var config = new ConfigResult
        {
            WriteHeader = true,
            Header = @"line1
line2",
            Exclude = new List<string> {"Dir1", "Dir2"},
            ReadOnly = true,
            LinkFormat = LinkFormat.Tfs,
            UrlsAsSnippets = new List<string> {"Url1", "Url2"},
            TocLevel = 5,
            MaxWidth = 80
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

    public LogBuilderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}