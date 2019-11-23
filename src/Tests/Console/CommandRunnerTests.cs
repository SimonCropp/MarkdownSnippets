using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class CommandRunnerTests :
    VerifyBase
{
    string? targetDirectory;
    ConfigInput? configInput;

    [Fact]
    public Task Empty()
    {
        CommandRunner.RunCommand(Capture);
        return VerifyResult();
    }

    [Fact]
    public Task SingleUnNamedArg()
    {
        CommandRunner.RunCommand(Capture, "dir");
        return VerifyResult();
    }

    [Fact]
    public Task Header()
    {
        CommandRunner.RunCommand(Capture, "--header", "the header");
        return VerifyResult();
    }

    [Fact]
    public Task WriteHeader()
    {
        CommandRunner.RunCommand(Capture, "--write-header", "false");
        return VerifyResult();
    }

    [Fact]
    public Task ReadOnlyShort()
    {
        CommandRunner.RunCommand(Capture, "-r");
        return VerifyResult();
    }

    [Fact]
    public Task ReadOnlyLong()
    {
        CommandRunner.RunCommand(Capture, "--readonly");
        return VerifyResult();
    }

    [Fact]
    public Task LinkFormatShort()
    {
        CommandRunner.RunCommand(Capture, "-l", "tfs");
        return VerifyResult();
    }

    [Fact]
    public Task LinkFormatLong()
    {
        CommandRunner.RunCommand(Capture, "--link-format", "tfs");
        return VerifyResult();
    }

    [Fact]
    public Task TargetDirectoryShort()
    {
        CommandRunner.RunCommand(Capture, "-t", "../");
        return VerifyResult();
    }

    [Fact]
    public Task TargetDirectoryLong()
    {
        CommandRunner.RunCommand(Capture, "--target-directory", "../");
        return VerifyResult();
    }

    [Fact]
    public Task TocLevelLong()
    {
        CommandRunner.RunCommand(Capture, "--toc-level", "5");
        return VerifyResult();
    }

    [Fact]
    public Task ExcludeShort()
    {
        CommandRunner.RunCommand(Capture, "-e", "dir");
        return VerifyResult();
    }

    [Fact]
    public Task ExcludeMultiple()
    {
        CommandRunner.RunCommand(Capture, "-e", "dir1:dir2");
        return VerifyResult();
    }

    [Fact]
    public void ExcludeDuplicates()
    {
        Assert.Throws<CommandLineException>(() => CommandRunner.RunCommand(Capture, "-e", "dir:dir"));
    }

    [Fact]
    public void ExcludeWhitespace()
    {
        Assert.Throws<CommandLineException>(() => CommandRunner.RunCommand(Capture, "-e", ": :"));
    }

    [Fact]
    public Task ExcludeLong()
    {
        CommandRunner.RunCommand(Capture, "--exclude", "dir");
        return VerifyResult();
    }

    [Fact]
    public Task UrlsAsSnippetsShort()
    {
        CommandRunner.RunCommand(Capture, "-u", "url");
        return VerifyResult();
    }

    [Fact]
    public void UrlsAsSnippetsMultiple()
    {
        CommandRunner.RunCommand(Capture, "-u", "url1:url2");
        VerifyResult();
    }

    [Fact]
    public void UrlsAsSnippetsDuplicates()
    {
        Assert.Throws<CommandLineException>(() => CommandRunner.RunCommand(Capture, "-u", "url:url"));
    }

    [Fact]
    public void UrlsAsSnippetsWhitespace()
    {
        Assert.Throws<CommandLineException>(() => CommandRunner.RunCommand(Capture, "-u", ": :"));
    }

    [Fact]
    public Task UrlsAsSnippetsLong()
    {
        CommandRunner.RunCommand(Capture, "--urls-as-snippets", "url");
        return VerifyResult();
    }

    void Capture(string targetDirectory, ConfigInput configInput)
    {
        this.targetDirectory = targetDirectory;
        this.configInput = configInput;
    }

    Task VerifyResult()
    {
        return Verify(
            new
            {
                targetDirectory,
                configInput
            });
    }


    public CommandRunnerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}