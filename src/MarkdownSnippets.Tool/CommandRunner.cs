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
                invoke(firstArg, new ConfigInput());
                return;
            }
        }

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(
                options =>
                {
                    ValidateAndApplyDefaults(options);
                    var configInput = new ConfigInput
                    {
                        ReadOnly = options.ReadOnly,
                        WriteHeader = options.WriteHeader,
                        LinkFormat = options.LinkFormat,
                        TocLevel = options.TocLevel,
                        Exclude = options.Exclude.ToList(),
                        TocExcludes = options.TocExcludes.ToList(),
                        UrlsAsSnippets = options.UrlsAsSnippets.ToList()
                    };
                    invoke(options.TargetDirectory, configInput);
                });
    }

    static void ValidateAndApplyDefaults(Options options)
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
            if (!Directory.Exists(options.TargetDirectory))
            {
                throw new CommandLineException("target-directory does not exist.");
            }
            options.TargetDirectory = Path.GetFullPath(options.TargetDirectory);
        }

        if (options.TocLevel <= 0)
        {
            throw new CommandLineException("toc-level must be positive.");
        }
        ValidateItems("exclude", options.Exclude);
        ValidateItems("toc-excludes", options.TocExcludes);
        ValidateItems("urls-as-snippets", options.UrlsAsSnippets);
    }

    static void ValidateItems(string name, IList<string> items)
    {
        if (items.Distinct().Count() != items.Count)
        {
            throw new CommandLineException($"duplicates found in {name}.");
        }

        if (items.Any(string.IsNullOrWhiteSpace))
        {
            throw new CommandLineException($"Empty items found in `{name}`.");
        }
    }
}