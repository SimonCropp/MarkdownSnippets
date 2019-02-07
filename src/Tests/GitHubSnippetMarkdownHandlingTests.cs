using System.Collections.Generic;
using System.IO;
using System.Text;
using ApprovalTests;
using CaptureSnippets;
using Xunit;

public class GitHubSnippetMarkdownHandlingTests : TestBase
{
    [Fact]
    public void AppendGroup()
    {
        var builder = new StringBuilder();
        var snippets = new List<Snippet> {Snippet.Build(1, 2, "theValue", "thekey", "thelanguage", "thePath")};
        var gitHubSnippetMarkdownHandling = new GitHubSnippetMarkdownHandling(GitRepoDirectoryFinder.FindForFilePath());
        using (var writer = new StringWriter(builder))
        {
            gitHubSnippetMarkdownHandling.AppendGroup("key1", snippets, writer);
        }

        Approvals.Verify(builder.ToString());
    }
}