using System.IO;
using System.Linq;
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
        processor.IncludeMdFiles(Directory.EnumerateFiles(Path.Combine(root, "docs"),"*.source.md").ToArray());
        processor.IncludeSnippetsFrom("src/Tests/Snippets");
        processor.Run();
    }

    [Fact]
    public void ReadOnly()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Readonly");
        var processor = new DirectoryMarkdownProcessor(root, scanForSnippets: false, writeHeader: false);
        processor.IncludeSnippets(
            SnippetBuild("snippet1"),
            SnippetBuild("snippet2")
        );
        processor.Run();

        Assert.True(new FileInfo(Path.Combine(root, "one.md")).IsReadOnly);
    }

    [Fact]
    public void Convention()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Convention");
        var processor = new DirectoryMarkdownProcessor(root, scanForSnippets: false, writeHeader: false);
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

    static Snippet SnippetBuild(string key)
    {
        return Snippet.Build(
            language: ".cs",
            startLine: 1,
            endLine: 2,
            value: "the code from " + key,
            key: key,
            path: null);
    }

    public DirectoryMarkdownProcessorTests(ITestOutputHelper output) :
        base(output)
    {
    }
}