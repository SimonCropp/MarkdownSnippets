using System.IO;
using System.Linq;
using CaptureSnippets;
using Xunit;

public class GitHubMarkdownProcessorTests : TestBase
{
    [Fact]
    public void Run()
    {
        var root = GitRepoDirectoryFinder.FindForFilePath();

        var files = Directory.EnumerateFiles(Path.Combine(root, "src/Tests/Snippets"), "*.cs");
        GitHubMarkdownProcessor.Run(root, files.ToList());
    }
}