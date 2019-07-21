using System.Collections.Generic;
using System.IO;
using System.Text;
using ApprovalTests;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class SnippetMarkdownHandlingTests :
    TestBase
{
    [Fact]
    public void AppendGroup()
    {
        var builder = new StringBuilder();
        var snippets = new List<Snippet> {Snippet.Build(1, 2, "theValue", "thekey", "thelanguage", "thePath")};
        var gitHubSnippetMarkdownHandling = new SnippetMarkdownHandling(GitRepoDirectoryFinder.FindForFilePath(), LinkFormat.GitHub);
        using (var writer = new StringWriter(builder))
        {
            gitHubSnippetMarkdownHandling.AppendGroup("key1", snippets, writer.WriteLine);
        }

        Approvals.Verify(builder.ToString());
    }

    public SnippetMarkdownHandlingTests(ITestOutputHelper output) :
        base(output)
    {
    }
}