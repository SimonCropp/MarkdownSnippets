using System.IO;
using System.Text;
using ApprovalTests;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class DirectoryMarkdownProcessorTests :
    TestBase
{
    [Fact]
    public void Run()
    {
        var root = GitRepoDirectoryFinder.FindForFilePath();

        var processor = new DirectoryMarkdownProcessor(root, false, false);
        processor.IncludeMdFiles(Path.Combine(root, "readme.source.md"));
        processor.IncludeSnippetsFrom("src/Tests/Snippets");
        processor.Run();
    }

    [Fact]
    public void Convention()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Convention");
        var processor = new DirectoryMarkdownProcessor(root, scanForSnippets: false)
        {
            WriteHeader = false
        };
        processor.IncludeSnippets(
            SnippetBuild("snippet1"),
            SnippetBuild("snippet2")
        );
        processor.Run();

        var builder = new StringBuilder();
        foreach (var file in Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories))
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

    public DirectoryMarkdownProcessorTests(ITestOutputHelper output) :
        base(output)
    {
    }
}