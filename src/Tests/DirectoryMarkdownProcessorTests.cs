using MarkdownSnippets;

[UsesVerify]
public class DirectoryMarkdownProcessorTests
{
    [Fact]
    public void Run()
    {
        var root = GitRepoDirectoryFinder.FindForFilePath();

        var processor = new DirectoryMarkdownProcessor(
            targetDirectory: root,
            directoryIncludes: path => !path.Contains("IncludeFileFinder") &&
                                       !path.Contains("DirectoryMarkdownProcessor") &&
                                       !DefaultDirectoryExclusions.ShouldExcludeDirectory(path),
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true,
            tocLevel: 1,
            tocExcludes: new List<string> {"Icon", "Credits", "Release Notes"});
        processor.Run();
    }

    [Fact]
    public Task InPlaceOverwriteExists()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/InPlaceOverwriteExists");
        var processor = new DirectoryMarkdownProcessor(
            root,
            convention: DocumentConvention.InPlaceOverwrite,
            writeHeader: false,
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.AddSnippets(SnippetBuild("snippet1", "thePath"));
        processor.Run();

        var fileInfo = new FileInfo(Path.Combine(root, "file.md"));
        return VerifyFile(fileInfo);
    }

    [Fact]
    public Task InPlaceOverwriteNotExists()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/InPlaceOverwriteNotExists");
        var processor = new DirectoryMarkdownProcessor(
            root,
            convention: DocumentConvention.InPlaceOverwrite,
            writeHeader: false,
            readOnly: false,
            newLine: "\r",
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.AddSnippets(SnippetBuild("snippet1", "thePath"));
        processor.Run();

        var fileInfo = new FileInfo(Path.Combine(root, "file.md"));
        return VerifyFile(fileInfo);
    }

    [Fact]
    public Task InPlaceOverwriteUrlSnippet()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/InPlaceOverwriteUrlSnippet");
        var processor = new DirectoryMarkdownProcessor(
            root,
            convention: DocumentConvention.InPlaceOverwrite,
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task InPlaceOverwriteUrlInclude()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/InPlaceOverwriteUrlInclude");
        var processor = new DirectoryMarkdownProcessor(
            root,
            convention: DocumentConvention.InPlaceOverwrite,
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task InPlaceOverwriteWithFileSnippetMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/InPlaceOverwriteWithFileSnippetMissing");
        var processor = new DirectoryMarkdownProcessor(
            root,
            convention: DocumentConvention.InPlaceOverwrite,
            writeHeader: false,
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true,
            treatMissingAsWarning: true);

        processor.Run();

        var fileInfo = new FileInfo(Path.Combine(root, "file.md"));
        return VerifyFile(fileInfo);
    }

    [Fact]
    public void ReadOnly()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Readonly");
        try
        {
            var processor = new DirectoryMarkdownProcessor(root,
                writeHeader: false,
                readOnly: true,
                newLine: "\r",
                directoryIncludes: _ => true,
                markdownDirectoryIncludes: _ => true,
                snippetDirectoryIncludes: _ => true);
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
    public Task FileSnippetMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/FileSnippetMissing");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            newLine: "\n",
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        return Throws(() => processor.Run());
    }

    [Fact]
    public Task UrlSnippetMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/UrlSnippetMissing");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            newLine: "\n",
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        return Throws(() => processor.Run());
    }

    [Fact]
    public Task ValidationErrors()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/ValidationErrors");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            validateContent: true,
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        return Throws(() => processor.Run());
    }

    [Fact]
    public Task UrlIncludeMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/UrlIncludeMissing");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            newLine: "\n",
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        return Throws(() => processor.Run());
    }

    [Fact]
    public Task UrlSnippet()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/UrlSnippet");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            newLine: "\r",
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task BinaryFileSnippet()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/BinaryFileSnippet");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            newLine: "\r",
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task FileSnippetWithWhiteSpace()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/FileSnippetWithWhiteSpace");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task FileSnippet()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/FileSnippet");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task MixedCaseInclude()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/MixedCaseInclude");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            newLine: "\r",
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task ExplicitFileInclude()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/ExplicitFileInclude");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task ExplicitFileIncludeWithMergedSnippet()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/ExplicitFileIncludeWithMergedSnippet");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            newLine: "\r",
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.AddSnippets(SnippetBuild("snippet1"));
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task ExplicitFileIncludeWithSnippetAtEnd()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/ExplicitFileIncludeWithSnippetAtEnd");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            newLine: "\r",
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.AddSnippets(SnippetBuild("snippet1"));
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task UrlInclude()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/UrlInclude");
        var processor = new DirectoryMarkdownProcessor(root,
            writeHeader: false,
            newLine: "\r",
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
        processor.Run();

        var result = Path.Combine(root, "one.md");

        return Verify(File.ReadAllTextAsync(result));
    }

    [Fact]
    public Task Convention()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Convention");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            newLine: "\r",
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true);
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

        return Verify(builder.ToString());
    }

    [Fact]
    public void MustErrorByDefaultWhenIncludesAreMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/MissingInclude");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true,
            newLine: "\n");
        Assert.Throws<MissingIncludesException>(() => processor.Run());
    }

    [Fact]
    public void MustNotErrorForMissingIncludesIfConfigured()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/MissingInclude");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            treatMissingAsWarning: true,
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true,
            newLine: "\n");
        processor.Run();
    }

    [Fact]
    public void MustErrorByDefaultWhenSnippetsAreMissing()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Convention");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true,
            newLine: "\n");
        Assert.Throws<MissingSnippetsException>(() => processor.Run());
    }

    [Fact]
    public void MustNotErrorForMissingSnippetsIfConfigured()
    {
        var root = Path.GetFullPath("DirectoryMarkdownProcessor/Convention");
        var processor = new DirectoryMarkdownProcessor(
            root,
            writeHeader: false,
            treatMissingAsWarning: true,
            directoryIncludes: _ => true,
            markdownDirectoryIncludes: _ => true,
            snippetDirectoryIncludes: _ => true,
            newLine: "\n");
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