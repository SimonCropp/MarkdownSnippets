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

        var processor = new DirectoryMarkdownProcessor(
            targetDirectory: root,
            tocLevel: 1,
            tocExcludes: new List<string>
            {
                "Icon",
                "Credits",
                "Release Notes"
            },
            directoryFilter: path =>
                !path.Contains("IncludeFileFinder") &&
                !path.Contains("DirectoryMarkdownProcessor"));
        processor.Run();
    }

    [Fact]
    public Task InPlaceOverwriteExists()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/InPlaceOverwriteExists");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            convention: DocumentConvention.InPlaceOverwrite);
        processor.AddSnippets(SnippetBuild("snippet1"));
        processor.Run();

        var fileInfo = new FileInfo(Path.Combine(root, "file.md"));
        return Verifier.VerifyFile(fileInfo);
    }

    [Fact]
    public Task InPlaceOverwriteNotExists()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/InPlaceOverwriteNotExists");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            readOnly: false,
            convention: DocumentConvention.InPlaceOverwrite);
        processor.AddSnippets(SnippetBuild("snippet1"));
        processor.Run();

        var fileInfo = new FileInfo(Path.Combine(root, "file.md"));
        return Verifier.VerifyFile(fileInfo);
    }

    [Fact]
    public void ReadOnly()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Readonly");
        try
        {
            var processor = new DirectoryMarkdownProcessor(
                root,
                writeHeader: false,
                readOnly: true);
            processor.AddSnippets(
                SnippetBuild("snippet1"),
                SnippetBuild("snippet2")
            );
            processor.Run();

            var fileInfo = new FileInfo(Path.Combine(root, "one.md"));
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
    public Task UrlSnippetMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/UrlSnippetMissing");
        var processor = new DirectoryMarkdownProcessor(root, writeHeader: false);
        var exception = Assert.Throws<MissingSnippetsException>(() => processor.Run());
        return Verifier.Verify(
            new
            {
                exception.Missing,
                exception.Message
            });
    }

    [Fact]
    public Task ValidationErrors()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/ValidationErrors");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            validateContent: true);
        var exception = Assert.Throws<ContentValidationException>(() => processor.Run());
        return Verifier.Verify(
            new
            {
                exception.Errors,
                exception.Message
            });
    }

    [Fact]
    public Task UrlIncludeMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/UrlIncludeMissing");
        var processor = new DirectoryMarkdownProcessor(root, writeHeader: false);
        var exception = Assert.Throws<MissingIncludesException>(() => processor.Run());
        return Verifier.Verify(
            new
            {
                exception.Missing,
                exception.Message
            });
    }

    [Fact]
    public Task UrlSnippet()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/UrlSnippet");
        var processor = new DirectoryMarkdownProcessor(root, writeHeader: false);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verifier.Verify(File.ReadAllText(result));
    }

    [Fact]
    public Task UrlInclude()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/UrlInclude");
        var processor = new DirectoryMarkdownProcessor(root, writeHeader: false);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verifier.Verify(File.ReadAllText(result));
    }

    [Fact]
    public Task NonMd()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/NonMd");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            documentExtensions: new List<string> {"txt"});
        processor.AddSnippets(
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

        return Verifier.Verify(builder.ToString());
    }

    [Fact]
    public Task Convention()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Convention");
        var processor = new DirectoryMarkdownProcessor(root, writeHeader: false);
        processor.AddSnippets(
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

        return Verifier.Verify(builder.ToString());
    }

    //[Fact]
    //public Task EmptyDocumentExtensions()
    //{
    //    var root = Path.GetFullPath("DirectoryMarkdownProcessor/Convention");
    //    var exception = Assert.Throws<ArgumentException>(() => new DirectoryMarkdownProcessor(root, documentExtensions: new List<string>()));
    //    return Verifier.Verify(exception.Message);
    //}

    [Fact]
    public void MustErrorByDefaultWhenIncludesAreMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/MissingInclude");
        var processor = new DirectoryMarkdownProcessor(root, writeHeader: false);
        Assert.Throws<MissingIncludesException>(() => processor.Run());
    }

    [Fact]
    public void MustNotErrorForMissingIncludesIfConfigured()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/MissingInclude");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            treatMissingAsWarning: true);
        processor.Run();
    }

    [Fact]
    public void MustErrorByDefaultWhenSnippetsAreMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Convention");
        var processor = new DirectoryMarkdownProcessor(root, writeHeader: false);
        Assert.Throws<MissingSnippetsException>(() => processor.Run());
    }

    [Fact]
    public void MustNotErrorForMissingSnippetsIfConfigured()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Convention");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            treatMissingAsWarning: true);
        processor.Run();
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
}