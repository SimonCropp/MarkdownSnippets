using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MarkdownSnippets;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class DirectoryMarkdownProcessorTests
{
    [Fact]
    public void Run()
    {
        var root = GitRepoDirectoryFinder.FindForFilePath();

        DirectoryMarkdownProcessor processor = new(
            targetDirectory: root,
            shouldIncludeDirectory: path =>
                !path.Contains("IncludeFileFinder") &&
                !path.Contains("DirectoryMarkdownProcessor") &&
                !DefaultDirectoryExclusions.ShouldExcludeDirectory(path),
            tocLevel: 1,
            tocExcludes: new List<string>
            {
                "Icon",
                "Credits",
                "Release Notes"
            });
        processor.Run();
    }

    [Fact]
    public Task InPlaceOverwriteExists()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/InPlaceOverwriteExists");
        DirectoryMarkdownProcessor processor = new(
            root,
            convention: DocumentConvention.InPlaceOverwrite,
            writeHeader: false,
            shouldIncludeDirectory: _ => true);
        processor.AddSnippets(SnippetBuild("snippet1", "thePath"));
        processor.Run();

        FileInfo fileInfo = new(Path.Combine(root, "file.md"));
        return Verifier.VerifyFile(fileInfo);
    }

    [Fact]
    public Task InPlaceOverwriteNotExists()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/InPlaceOverwriteNotExists");
        DirectoryMarkdownProcessor processor = new(
            root,
            convention: DocumentConvention.InPlaceOverwrite,
            writeHeader: false,
            readOnly: false,
            newLine: "\r",
            shouldIncludeDirectory: _ => true);
        processor.AddSnippets(SnippetBuild("snippet1", "thePath"));
        processor.Run();

        FileInfo fileInfo = new(Path.Combine(root, "file.md"));
        return Verifier.VerifyFile(fileInfo);
    }

    [Fact]
    public Task InPlaceOverwriteUrlSnippet()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/InPlaceOverwriteUrlSnippet");
        DirectoryMarkdownProcessor processor = new(
            root,
            convention: DocumentConvention.InPlaceOverwrite,
            shouldIncludeDirectory: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verifier.Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task InPlaceOverwriteUrlInclude()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/InPlaceOverwriteUrlInclude");
        DirectoryMarkdownProcessor processor = new(
            root,
            convention: DocumentConvention.InPlaceOverwrite,
            shouldIncludeDirectory: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verifier.Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public void ReadOnly()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Readonly");
        try
        {
            DirectoryMarkdownProcessor processor = new(
                root,
                writeHeader: false,
                readOnly: true,
                newLine: "\r",
                shouldIncludeDirectory: _ => true);
            processor.AddSnippets(
                SnippetBuild("snippet1"),
                SnippetBuild("snippet2")
            );
            processor.Run();

            FileInfo fileInfo = new(Path.Combine(root, "one.md"));
            Assert.True(fileInfo.IsReadOnly);
        }
        finally
        {
            foreach (var file in Directory.EnumerateFiles(root))
            {
                FileEx.ClearReadOnly(file);
            }
        }
    }

    [Fact]
    public Task FileSnippetMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/FileSnippetMissing");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            newLine: "\n",
            shouldIncludeDirectory: _ => true);
        return Verifier.Throws(() => processor.Run());
    }

    [Fact]
    public Task UrlSnippetMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/UrlSnippetMissing");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            newLine: "\n",
            shouldIncludeDirectory: _ => true);
        return Verifier.Throws(() => processor.Run());
    }

    [Fact]
    public Task ValidationErrors()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/ValidationErrors");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            validateContent: true,
            shouldIncludeDirectory: _ => true);
        return Verifier.Throws(() => processor.Run());
    }

    [Fact]
    public Task UrlIncludeMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/UrlIncludeMissing");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            newLine: "\n",
            shouldIncludeDirectory: _ => true);
        return Verifier.Throws(() => processor.Run());
    }

    [Fact]
    public Task UrlSnippet()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/UrlSnippet");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            newLine: "\r",
            shouldIncludeDirectory: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verifier.Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task BinaryFileSnippet()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/BinaryFileSnippet");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            newLine: "\r",
            shouldIncludeDirectory: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verifier.Verify(File.ReadAllTextAsync(result));
    }


    [Fact]
    public Task FileSnippet()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/FileSnippet");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            shouldIncludeDirectory: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verifier.Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task MixedCaseInclude()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/MixedCaseInclude");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            newLine: "\r",
            shouldIncludeDirectory: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verifier.Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task ExplicitFileInclude()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/ExplicitFileInclude");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            shouldIncludeDirectory: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verifier.Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task ExplicitFileIncludeWithMergedSnippet()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/ExplicitFileIncludeWithMergedSnippet");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            newLine: "\r",
            shouldIncludeDirectory: _ => true);
        processor.AddSnippets(SnippetBuild("snippet1"));
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verifier.Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task ExplicitFileIncludeWithSnippetAtEnd()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/ExplicitFileIncludeWithSnippetAtEnd");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            newLine: "\r",
            shouldIncludeDirectory: _ => true);
        processor.AddSnippets(SnippetBuild("snippet1"));
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verifier.Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task UrlInclude()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/UrlInclude");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            newLine: "\r",
            shouldIncludeDirectory: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verifier.Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task Convention()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Convention");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            newLine: "\r",
            shouldIncludeDirectory: _ => true);
        processor.AddSnippets(
            SnippetBuild("snippet1"),
            SnippetBuild("snippet2")
        );
        processor.Run();

        StringBuilder builder = new();
        foreach (var file in Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories))
        {
            builder.AppendLine(file.Replace(root, ""));
            builder.AppendLine(File.ReadAllText(file));
            builder.AppendLine();
        }

        return Verifier.Verify(builder.ToString());
    }

    [Fact]
    public void MustErrorByDefaultWhenIncludesAreMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/MissingInclude");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            shouldIncludeDirectory: _ => true);
        Assert.Throws<MissingIncludesException>(() => processor.Run());
    }

    [Fact]
    public void MustNotErrorForMissingIncludesIfConfigured()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/MissingInclude");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            treatMissingAsWarning: true,
            shouldIncludeDirectory: _ => true);
        processor.Run();
    }

    [Fact]
    public void MustErrorByDefaultWhenSnippetsAreMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Convention");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            shouldIncludeDirectory: _ => true);
        Assert.Throws<MissingSnippetsException>(() => processor.Run());
    }

    [Fact]
    public void MustNotErrorForMissingSnippetsIfConfigured()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Convention");
        DirectoryMarkdownProcessor processor = new(
            root,
            writeHeader: false,
            treatMissingAsWarning: true,
            shouldIncludeDirectory: _ => true);
        processor.Run();
    }

    static Snippet SnippetBuild(string key, string? path = null)
    {
        return Snippet.Build(
            language: "cs",
            startLine: 1,
            endLine: 2,
            value: "the code from " + key,
            key: key,
            path: path);
    }
}