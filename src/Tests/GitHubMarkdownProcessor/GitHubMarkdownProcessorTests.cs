using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ApprovalTests;
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
        GitHubMarkdownProcessor.Run(root,snippets, new List<string>(),false);

         var builder = new StringBuilder();
         foreach (var file in Directory.EnumerateFiles(root,"*.*",SearchOption.AllDirectories))
         {
             builder.AppendLine(file.Replace(root, ""));
             builder.AppendLine(File.ReadAllText(file));
             builder.AppendLine();
        }
        Approvals.Verify(builder.ToString());
    }  

    Snippet SnippetBuild(string key)
    {
        return Snippet.Build(
            language: ".cs",
            startLine: 1,
            endLine: 2,
            value: "the code from "+ key,
            key: key,
            path: null);
    }

    public GitHubMarkdownProcessorTests(ITestOutputHelper output) :
        base(output)
    {
    }
}