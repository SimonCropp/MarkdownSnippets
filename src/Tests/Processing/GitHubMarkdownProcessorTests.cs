using System.IO;
using System.Linq;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class GitHubMarkdownProcessorTests :
    TestBase
{
    [Fact]
    public void Run()
    {
        var root = GitRepoDirectoryFinder.FindForFilePath();

        var files = Directory.EnumerateFiles(Path.Combine(root, "src/Tests/Snippets"), "*.cs");
        GitHubMarkdownProcessor.Run(root, files.ToList());
    }

    public GitHubMarkdownProcessorTests(ITestOutputHelper output) :
        base(output)
    {
    }
}