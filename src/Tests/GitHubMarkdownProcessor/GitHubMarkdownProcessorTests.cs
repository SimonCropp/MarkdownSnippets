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
        var snippets = new List<Snippet>
        {
            SnippetBuild("snippet1"),
            SnippetBuild("snippet2"),
        };
        GitHubMarkdownProcessor.Run(root, new List<string>(),snippets);
    }  

    Snippet SnippetBuild(string key)
    {
        return Snippet.Build(
            language: ".cs",
            startLine: 1,
            endLine: 2,
            value: "the code from "+ key,
            key: key,
            path: "thePath");
    }

    public GitHubMarkdownProcessorTests(ITestOutputHelper output) :
        base(output)
    {
    }
}