using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

[UsesVerify]
public class CommandRunnerTests
{
    string? targetDirectory;
    ConfigInput? configInput;

    [Fact]
    public async Task Empty()
    {
        await CommandRunner.RunCommand(Capture);
        await VerifyResult();
    }

    [Fact]
    public async Task SingleUnNamedArg()
    {
        await CommandRunner.RunCommand(Capture, "dir");
        await VerifyResult();
    }

    [Fact]
    public async Task Header()
    {
        await CommandRunner.RunCommand(Capture, "--header", "the header");
        await VerifyResult();
    }

    [Fact]
    public async Task UrlPrefix()
    {
        await CommandRunner.RunCommand(Capture, "--urlPrefix", "the prefix");
        await VerifyResult();
    }

    [Fact]
    public async Task WriteHeader()
    {
        await CommandRunner.RunCommand(Capture, "--write-header", "false");
        await VerifyResult();
    }

    [Fact]
    public async Task ValidateContentShort()
    {
        await CommandRunner.RunCommand(Capture, "-v", "false");
        await VerifyResult();
    }

    [Fact]
    public async Task ValidateContentLong()
    {
        await CommandRunner.RunCommand(Capture, "--validate-content", "false");
        await VerifyResult();
    }

    [Fact]
    public async Task ReadOnlyShort()
    {
        await CommandRunner.RunCommand(Capture, "-r", "false");
        await VerifyResult();
    }

    [Fact]
    public async Task ReadOnlyLong()
    {
        await CommandRunner.RunCommand(Capture, "--readonly", "false");
        await VerifyResult();
    }

    [Fact]
    public async Task LinkFormatShort()
    {
        await CommandRunner.RunCommand(Capture, "-l", "tfs");
        await VerifyResult();
    }

    [Fact]
    public async Task LinkFormatLong()
    {
        await CommandRunner.RunCommand(Capture, "--link-format", "tfs");
        await VerifyResult();
    }

    [Fact]
    public async Task TargetDirectoryShort()
    {
        await CommandRunner.RunCommand(Capture, "-t", "../");
        await VerifyResult();
    }

    [Fact]
    public async Task TargetDirectoryLong()
    {
        await CommandRunner.RunCommand(Capture, "--target-directory", "../");
        await VerifyResult();
    }

    [Fact]
    public async Task MaxWidthLong()
    {
        await CommandRunner.RunCommand(Capture, "--max-width", "5");
        await VerifyResult();
    }

    [Fact]
    public async Task TocLevelLong()
    {
        await CommandRunner.RunCommand(Capture, "--toc-level", "5");
        await VerifyResult();
    }

    [Fact]
    public async Task ExcludeShort()
    {
        await CommandRunner.RunCommand(Capture, "-e", "dir");
        await VerifyResult();
    }

    [Fact]
    public async Task ExcludeMultiple()
    {
        await CommandRunner.RunCommand(Capture, "-e", "dir1:dir2");
        await VerifyResult();
    }

    [Fact]
    public Task ExcludeDuplicates()
    {
        return Assert.ThrowsAsync<CommandLineException>(() => CommandRunner.RunCommand(Capture, "-e", "dir:dir"));
    }

    [Fact]
    public Task ExcludeWhitespace()
    {
        return Assert.ThrowsAsync<CommandLineException>(() => CommandRunner.RunCommand(Capture, "-e", ": :"));
    }

    [Fact]
    public async Task ExcludeLong()
    {
        await CommandRunner.RunCommand(Capture, "--exclude", "dir");
        await VerifyResult();
    }

    [Fact]
    public async Task UrlsAsSnippetsShort()
    {
        await CommandRunner.RunCommand(Capture, "-u", "url");
        await VerifyResult();
    }

    [Fact]
    public async Task UrlsAsSnippetsMultiple()
    {
        await CommandRunner.RunCommand(Capture, "-u", "url1 url2");
        await VerifyResult();
    }

    [Fact]
    public Task UrlsAsSnippetsDuplicates()
    {
        return Assert.ThrowsAsync<CommandLineException>(() => CommandRunner.RunCommand(Capture, "-u", "url url"));
    }

    [Fact]
    public Task UrlsAsSnippetsWhitespace()
    {
        return Assert.ThrowsAsync<CommandLineException>(() => CommandRunner.RunCommand(Capture, "-u", ": :"));
    }

    [Fact]
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

    Task VerifyResult()
    {
        return Verifier.Verify(
            new
            {
                targetDirectory,
                configInput
            });
    }
}