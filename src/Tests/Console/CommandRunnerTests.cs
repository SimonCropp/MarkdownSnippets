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
    bool readOnly;

    [Fact]
    public void Empty()
    {
        CommandRunner.RunCommand(Capture);
        Assert.Equal(Environment.CurrentDirectory, targetDirectory);
        Assert.Empty(exclude);
        Assert.False(readOnly);
    }

    [Fact]
    public void SingleUnNamedArg()
    {
        CommandRunner.RunCommand(Capture, "dir");
        Assert.Equal("dir", targetDirectory);
        Assert.Empty(exclude);
        Assert.False(readOnly);
    }

    [Fact]
    public void ReadOnlyShort()
    {
        CommandRunner.RunCommand(Capture, "-r");
        Assert.Equal(Environment.CurrentDirectory, targetDirectory);
        Assert.Empty(exclude);
        Assert.True(readOnly);
    }

    [Fact]
    public void ReadOnlyLong()
    {
        CommandRunner.RunCommand(Capture, "--readonly");
        Assert.Equal(Environment.CurrentDirectory, targetDirectory);
        Assert.Empty(exclude);
        Assert.True(readOnly);
    }

    [Fact]
    public void TargetDirectoryShort()
    {
        CommandRunner.RunCommand(Capture, "-t", "dir");
        Assert.Equal(Path.GetFullPath("dir"), targetDirectory);
        Assert.False(readOnly);
    }

    [Fact]
    public void TargetDirectoryLong()
    {
        CommandRunner.RunCommand(Capture, "--target-directory", "dir");
        Assert.Equal(Path.GetFullPath("dir"), targetDirectory);
        Assert.False(readOnly);
    }

    [Fact]
    public void ExcludeShort()
    {
        CommandRunner.RunCommand(Capture, "-e", "dir");
        Assert.Equal("dir", exclude.Single());
        Assert.False(readOnly);
    }

    [Fact]
    public void ExcludeMultiple()
    {
        CommandRunner.RunCommand(Capture, "-e", "dir1:dir2");
        Assert.Equal(new List<string> {"dir1", "dir2"}, exclude);
        Assert.False(readOnly);
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
        Assert.False(readOnly);
    }

    void Capture(string targetDirectory, bool readOnly, List<string> exclude)
    {
        this.targetDirectory = targetDirectory;
        this.exclude = exclude;
        this.readOnly = readOnly;
    }

    public CommandRunnerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}