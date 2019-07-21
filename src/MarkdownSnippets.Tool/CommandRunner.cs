using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using MarkdownSnippets;

static class CommandRunner
{
    public static void RunCommand(Invoke invoke, params string[] args)
    {
        if (args.Length == 1)
        {
            var firstArg = args[0];
            if (!firstArg.StartsWith('-'))
            {
                invoke(firstArg, null, null, null, new List<string>());
                return;
            }
        }

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(
                options =>
                {
                    ApplyDefaults(options);
                    invoke(options.TargetDirectory, options.ReadOnly, options.WriteHeader, options.LinkFormat, options.Exclude.ToList());
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

        var exclude = options.Exclude;
        if (exclude.Distinct().Count() != exclude.Count)
        {
            throw new CommandLineException("Exclude contains duplicates");
        }

        if (exclude.Any(string.IsNullOrWhiteSpace))
        {
            throw new CommandLineException("Some excludes are empty");
        }
    }
}