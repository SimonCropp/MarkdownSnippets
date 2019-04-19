using System.Collections.Generic;
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

    [Fact]
    public void Convention()
    {
        var root = Path.GetFullPath("GitHubMarkdownProcessor/Convention");
        GitHubMarkdownProcessor.Run(root, new List<string>(),new List<Snippet> {SnippetBuild()});
    }  

    Snippet SnippetBuild()
    {
        return Snippet.Build(
            language: ".cs",
            startLine: 1,
            endLine: 2,
            value: "the code",
            key: "theKey",
            path: "thePath");
    }

    public GitHubMarkdownProcessorTests(ITestOutputHelper output) :
        base(output)
    {
    }
}