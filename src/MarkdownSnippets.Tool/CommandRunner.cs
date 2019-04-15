using System;
using System.IO;
using CommandLine;
using MarkdownSnippets;

static class CommandRunner
{
    public static void RunCommand(string[] args, Invoke invoke)
    {
        if (args.Length == 1)
        {
            var firstArg = args[0];
            if (!firstArg.StartsWith('-'))
            {
                invoke(firstArg);
                return;
            }
        }

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(
                options =>
                {
                    ApplyDefaults(options);
                    invoke(options.TargetDirectory);
                });
    }

    static void ApplyDefaults(Options options)
    {
        if (options.TargetDirectory == null)
        {
            options.TargetDirectory = Environment.CurrentDirectory;
            if (!GitRepoDirectoryFinder.IsInGitRepository(options.TargetDirectory))
            {
                throw new CommandLineException($"The current directory does no exist with a .git repository. Pass in a target directory instead. Current directory: {options.TargetDirectory}");
            }
        }
        else
        {
            options.TargetDirectory = Path.GetFullPath(options.TargetDirectory);
        }
    }
}