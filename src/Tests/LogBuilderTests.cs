using System.Collections.Generic;
using ApprovalTests;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class LogBuilderTests :
    XunitLoggingBase
{
    [Fact]
    public void BuildConfigLogMessage()
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
            TocLevel = 5
        };
        var message = LogBuilder.BuildConfigLogMessage("theRoot", config, "theConfigFilePath");
        Approvals.Verify(message);
    }

    [Fact]
    public void BuildConfigLogMessageMinimal()
    {
        var config = new ConfigResult();
        var message = LogBuilder.BuildConfigLogMessage("theRoot", config, "theConfigFilePath");
        Approvals.Verify(message);
    }

    public LogBuilderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}