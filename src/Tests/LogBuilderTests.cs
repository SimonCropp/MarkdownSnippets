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
            Exclude = new List<string> {"Dir1"},
            ReadOnly = true,
            LinkFormat = LinkFormat.Tfs,
            UrlsAsSnippets = new List<string> {"Url1"}
        };
        var message = LogBuilder.BuildConfigLogMessage("theRoot", config, "theConfigFilePath");
        Approvals.Verify(message);
    }

    public LogBuilderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}