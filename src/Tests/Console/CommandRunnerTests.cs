using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

public class CommandRunnerTests :
    XunitLoggingBase
{
    string targetDirectory;
    List<string> exclude;

    [Fact]
    public void Empty()
    {
        CommandRunner.RunCommand(Capture);
        Assert.Equal(Environment.CurrentDirectory, targetDirectory);
        Assert.Empty(exclude);
    }

    [Fact]
    public void SingleUnNamedArg()
    {
        CommandRunner.RunCommand(Capture, "dir");
        Assert.Equal("dir", targetDirectory);
        Assert.Empty(exclude);
    }

    [Fact]
    public void TargetDirectoryShort()
    {
        CommandRunner.RunCommand(Capture, "-t", "dir");
        Assert.Equal(Path.GetFullPath("dir"), targetDirectory);
    }

    [Fact]
    public void TargetDirectoryLong()
    {
        CommandRunner.RunCommand(Capture, "--target-directory", "dir");
        Assert.Equal(Path.GetFullPath("dir"), targetDirectory);
    }

    [Fact]
    public void ExcludeShort()
    {
        CommandRunner.RunCommand(Capture, "-e", "dir");
        Assert.Equal("dir", exclude.Single());
    }

    [Fact]
    public void ExcludeMultiple()
    {
        CommandRunner.RunCommand(Capture, "-e", "dir1:dir2");
        Assert.Equal(new List<string> {"dir1", "dir2"}, exclude);
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
        Assert.Equal("dir", exclude.Single());
    }

    void Capture(string targetDirectory, List<string> exclude)
    {
        this.targetDirectory = targetDirectory;
        this.exclude = exclude;
    }

    public CommandRunnerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}