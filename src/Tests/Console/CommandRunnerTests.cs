using System;
using System.Collections.Generic;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class CommandRunnerTests :
    XunitLoggingBase
{
    string targetDirectory;
    List<string> exclude;
    bool? readOnly;
    bool? writeHeader;

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
    public void TargetDirectoryShort()
    {
        CommandRunner.RunCommand(Capture, "-t", "dir");
        Verify();
    }

    [Fact]
    public void TargetDirectoryLong()
    {
        CommandRunner.RunCommand(Capture, "--target-directory", "dir");
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

    void Capture(string targetDirectory, bool? readOnly, bool? writeHeader, List<string> exclude)
    {
        this.targetDirectory = targetDirectory;
        this.exclude = exclude;
        this.readOnly = readOnly;
        this.writeHeader = writeHeader;
    }

    void Verify()
    {
        ObjectApprover.VerifyWithJson(new {targetDirectory, readOnly, writeHeader, exclude},s => s.Replace(Environment.CurrentDirectory, "TheTargetDirectory"));
    }

    public CommandRunnerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}