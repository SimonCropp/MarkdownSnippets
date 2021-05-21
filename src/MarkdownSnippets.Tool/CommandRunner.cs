using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using MarkdownSnippets;

static class CommandRunner
{
    public static Task RunCommand(Invoke invoke, params string[] args)
    {
        if (args.Length == 1)
        {
            var firstArg = args[0];
            if (!firstArg.StartsWith('-'))
            {
                return invoke(firstArg, new());
            }
        }

        return Parser.Default.ParseArguments<Options>(args)
            .WithParsedAsync(
                options =>
                {
                    ValidateAndApplyDefaults(options);
                    ConfigInput input = new()
                    {
                        ReadOnly = options.ReadOnly,
                        ValidateContent = options.ValidateContent,
                        WriteHeader = options.WriteHeader,
                        Header = options.Header,
                        UrlPrefix = options.UrlPrefix,
                        LinkFormat = options.LinkFormat,
                        TocLevel = options.TocLevel,
                        MaxWidth = options.MaxWidth,
                        ExcludeDirectories = options.ExcludeDirectories.ToList(),
                        ExcludeMarkdownDirectories = options.ExcludeMarkdownDirectories.ToList(),
                        ExcludeSnippetDirectories = options.ExcludeSnippetDirectories.ToList(),
                        TocExcludes = options.TocExcludes.ToList(),
                        UrlsAsSnippets = options.UrlsAsSnippets.ToList(),
                        TreatMissingAsWarning = options.TreatMissingAsWarning,
                        Convention = options.Convention,
                        HashSnippetAnchors = options.HashSnippetAnchors
                    };
                    return invoke(options.TargetDirectory!, input);
                });
    }

    static void ValidateAndApplyDefaults(Options options)
    {
        if (options.Exclude.Any())
        {
            throw new CommandLineException("`exclude` is obsolete. Use `exclude-directories`.");
        }

        if (options.Header != null && string.IsNullOrWhiteSpace(options.Header))
        {
            throw new CommandLineException("Empty Header is not allowed.");
        }

        if (options.UrlPrefix != null && string.IsNullOrWhiteSpace(options.UrlPrefix))
        {
            throw new CommandLineException("Empty UrlPrefix is not allowed.");
        }

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

        if (options.MaxWidth <= 0)
        {
            throw new CommandLineException("max-width must be positive.");
        }

        ValidateItems("exclude", options.ExcludeDirectories);
        ValidateItems("exclude-markdown-directories", options.ExcludeMarkdownDirectories);
        ValidateItems("exclude-snippet-directories", options.ExcludeSnippetDirectories);
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