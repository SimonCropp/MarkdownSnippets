using System;
using System.IO;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class CommandRunnerTests :
    XunitLoggingBase
{
    string targetDirectory;
    ConfigInput configInput;

    [Fact]
    public void Empty()
    {
        CommandRunner.RunCommand(Capture);
        Verify();
    }

    [Fact]
    public void SingleUnNamedArg()
    {
        CommandRunner.RunCommand(Capture, "dir");
        Verify();
    }

    [Fact]
    public void WriteHeaderShort()
    {
        CommandRunner.RunCommand(Capture, "-h", "false");
        Verify();
    }

    [Fact]
    public void WriteHeaderLong()
    {
        CommandRunner.RunCommand(Capture, "--write-header", "false");
        Verify();
    }

    [Fact]
    public void ReadOnlyShort()
    {
        CommandRunner.RunCommand(Capture, "-r");
        Verify();
    }

    [Fact]
    public void ReadOnlyLong()
    {
        CommandRunner.RunCommand(Capture, "--readonly");
        Verify();
    }

    [Fact]
    public void LinkFormatShort()
    {
        CommandRunner.RunCommand(Capture, "-l", "tfs");
        Verify();
    }

    [Fact]
    public void LinkFormatLong()
    {
        CommandRunner.RunCommand(Capture, "--link-format", "tfs");
        Verify();
    }

    [Fact]
    public void TargetDirectoryShort()
    {
        CommandRunner.RunCommand(Capture, "-t", "../");
        Verify();
    }

    [Fact]
    public void TargetDirectoryLong()
    {
        CommandRunner.RunCommand(Capture, "--target-directory", "../");
        Verify();
    }

    [Fact]
    public void TocLevelLong()
    {
        CommandRunner.RunCommand(Capture, "--toc-level", "5");
        Verify();
    }

    [Fact]
    public void ExcludeShort()
    {
        CommandRunner.RunCommand(Capture, "-e", "dir");
        Verify();
    }

    [Fact]
    public void ExcludeMultiple()
    {
        CommandRunner.RunCommand(Capture, "-e", "dir1:dir2");
        Verify();
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
    public void ExcludeLong()
    {
        CommandRunner.RunCommand(Capture, "--exclude", "dir");
        Verify();
    }

    [Fact]
    public void UrlsAsSnippetsShort()
    {
        CommandRunner.RunCommand(Capture, "-u", "url");
        Verify();
    }

    [Fact]
    public void UrlsAsSnippetsMultiple()
    {
        CommandRunner.RunCommand(Capture, "-u", "url1:url2");
        Verify();
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
    public void UrlsAsSnippetsLong()
    {
        CommandRunner.RunCommand(Capture, "--urls-as-snippets", "url");
        Verify();
    }

    void Capture(string targetDirectory, ConfigInput configInput)
    {
        this.targetDirectory = targetDirectory;
        this.configInput = configInput;
    }

    void Verify()
    {
        ObjectApprover.VerifyWithJson(
            new
            {
                targetDirectory,
                configInput
            },
            scrubber: s =>
            {
                return s.Replace(Environment.CurrentDirectory, "TheTargetDirectory")
                    .Replace(Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,"../")), "TheTargetDirectory");
            });
    }

    public CommandRunnerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}