using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

public class CommandRunnerTests :
    XunitLoggingBase
{
    string targetDirectory;

    [Fact]
    public void Empty()
    {
        CommandRunner.RunCommand(new string[] { }, Capture);
        Assert.Equal(Environment.CurrentDirectory, targetDirectory);
    }

    [Fact]
    public void SingleUnNamedArg()
    {
        CommandRunner.RunCommand(new[] {"dir"}, Capture);
        Assert.Equal("dir", targetDirectory);
    }

    [Fact]
    public void TargetDirectoryShort()
    {
        CommandRunner.RunCommand(new[] {"-t", "dir"}, Capture);
        Assert.Equal(Path.GetFullPath("dir"), targetDirectory);
    }

    [Fact]
    public void TargetDirectoryLong()
    {
        CommandRunner.RunCommand(new[] {"--target-directory", "dir"}, Capture);
        Assert.Equal(Path.GetFullPath("dir"), targetDirectory);
    }

    void Capture(string targetDirectory)
    {
        this.targetDirectory = targetDirectory;
    }

    public CommandRunnerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}