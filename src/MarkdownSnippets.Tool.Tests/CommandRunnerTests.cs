public class CommandRunnerTests
{
    string? targetDirectory;
    ConfigInput? configInput;

    [Test]
    public async Task Empty()
    {
        await CommandRunner.RunCommand(Capture);
        await VerifyResult();
    }

    [Test]
    public async Task SingleUnNamedArg()
    {
        await CommandRunner.RunCommand(Capture, "dir");
        await VerifyResult();
    }

    [Test]
    public async Task Header()
    {
        await CommandRunner.RunCommand(Capture, "--header", "the header");
        await VerifyResult();
    }

    [Test]
    public async Task UrlPrefix()
    {
        await CommandRunner.RunCommand(Capture, "--url-prefix", "the prefix");
        await VerifyResult();
    }

    [Test]
    public async Task WriteHeader()
    {
        await CommandRunner.RunCommand(Capture, "--write-header", "false");
        await VerifyResult();
    }

    [Test]
    public async Task OmitSnippetLinks()
    {
        await CommandRunner.RunCommand(Capture, "--omit-snippet-links", "true");
        await VerifyResult();
    }

    [Test]
    public async Task ValidateContentShort()
    {
        await CommandRunner.RunCommand(Capture, "-v", "false");
        await VerifyResult();
    }

    [Test]
    public async Task ValidateContentLong()
    {
        await CommandRunner.RunCommand(Capture, "--validate-content", "false");
        await VerifyResult();
    }

    [Test]
    public async Task ConventionShort()
    {
        await CommandRunner.RunCommand(Capture, "-c", "InPlaceOverwrite");
        await VerifyResult();
    }

    [Test]
    public async Task ConventionLong()
    {
        await CommandRunner.RunCommand(Capture, "--convention", "InPlaceOverwrite");
        await VerifyResult();
    }

    [Test]
    public async Task ReadOnlyShort()
    {
        await CommandRunner.RunCommand(Capture, "-r", "false");
        await VerifyResult();
    }

    [Test]
    public async Task ReadOnlyLong()
    {
        await CommandRunner.RunCommand(Capture, "--read-only", "false");
        await VerifyResult();
    }

    [Test]
    public async Task LinkFormatShort()
    {
        await CommandRunner.RunCommand(Capture, "-l", "tfs");
        await VerifyResult();
    }

    [Test]
    public async Task LinkFormatLong()
    {
        await CommandRunner.RunCommand(Capture, "--link-format", "tfs");
        await VerifyResult();
    }

    [Test]
    public async Task TargetDirectoryShort()
    {
        await CommandRunner.RunCommand(Capture, "-t", "../../");
        await VerifyResult();
    }

    [Test]
    public async Task TargetDirectoryLong()
    {
        await CommandRunner.RunCommand(Capture, "--target-directory", "../../");
        await VerifyResult();
    }

    [Test]
    public async Task MaxWidthLong()
    {
        await CommandRunner.RunCommand(Capture, "--max-width", "5");
        await VerifyResult();
    }

    [Test]
    public async Task TocLevelLong()
    {
        await CommandRunner.RunCommand(Capture, "--toc-level", "5");
        await VerifyResult();
    }

    [Test]
    public async Task ExcludeShort()
    {
        await CommandRunner.RunCommand(Capture, "-e", "dir");
        await VerifyResult();
    }

    [Test]
    public async Task ExcludeMultiple()
    {
        await CommandRunner.RunCommand(Capture, "-e", "dir1:dir2");
        await VerifyResult();
    }

    [Test]
    public async Task ExcludeDuplicates() =>
        await Assert.ThrowsAsync<CommandLineException>(() => CommandRunner.RunCommand(Capture, "-e", "dir:dir"));

    [Test]
    public async Task ExcludeWhitespace() =>
        await Assert.ThrowsAsync<CommandLineException>(() => CommandRunner.RunCommand(Capture, "-e", ": :"));

    [Test]
    public async Task ExcludeLong()
    {
        await CommandRunner.RunCommand(Capture, "--exclude-directories", "dir");
        await VerifyResult();
    }

    [Test]
    public async Task ExcludeMarkdownDirectoriesLong()
    {
        await CommandRunner.RunCommand(Capture, "--exclude-markdown-directories", "dir");
        await VerifyResult();
    }

    [Test]
    public async Task ExcludeSnippetDirectoriesLong()
    {
        await CommandRunner.RunCommand(Capture, "--exclude-snippet-directories", "dir");
        await VerifyResult();
    }

    [Test]
    public async Task UrlsAsSnippetsShort()
    {
        await CommandRunner.RunCommand(Capture, "-u", "url");
        await VerifyResult();
    }

    [Test]
    public async Task UrlsAsSnippetsMultiple()
    {
        await CommandRunner.RunCommand(Capture, "-u", "url1 url2");
        await VerifyResult();
    }

    [Test]
    public async Task UrlsAsSnippetsDuplicates() =>
        await Assert.ThrowsAsync<CommandLineException>(() => CommandRunner.RunCommand(Capture, "-u", "url url"));

    [Test]
    public async Task UrlsAsSnippetsWhitespace() => await Assert.ThrowsAsync<CommandLineException>(() => CommandRunner.RunCommand(Capture, "-u", ": :"));

    [Test]
    public async Task UrlsAsSnippetsLong()
    {
        await CommandRunner.RunCommand(Capture, "--urls-as-snippets", "url");
        await VerifyResult();
    }

    Task Capture(string targetDirectory, ConfigInput configInput)
    {
        this.targetDirectory = targetDirectory;
        this.configInput = configInput;
        return Task.CompletedTask;
    }

    Task VerifyResult() =>
        Verify(
            new
            {
                targetDirectory,
                configInput
            });
}