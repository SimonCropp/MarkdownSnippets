using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using MarkdownSnippets;

class Program
{
    static void Main(string[] args)
    {
        GitHubMarkdownProcessor.Log = Console.WriteLine;
        if (args.Length == 1)
        {
            var firstArg = args[0];
            if (!firstArg.StartsWith('-'))
            {
                Inner(firstArg);
                return;
            }
        }
        CommandLine.Parser.Default.ParseArguments<Options>(args)
            .WithParsed(
                options =>
                {
                    ApplyDefaults(options);
                    Inner(options.TargetDirectory);
                });
    }
    
    static void Inner(string targetDirectory)
    {
        Console.WriteLine($"TargetDirectory: {targetDirectory}");
        if (!Directory.Exists(targetDirectory))
        {
            Console.WriteLine($"Target directory does not exist: {targetDirectory}");
            Environment.Exit(1);
        }
        try
        {
            GitHubMarkdownProcessor.Run(targetDirectory);
        }
        catch (SnippetReadingException exception)
        {
            Console.WriteLine($"Failed to read snippets: {exception.Message}");
            Environment.Exit(1);
        }
        catch (MissingSnippetsException exception)
        {
            Console.WriteLine($"Failed to process markdown: {exception.Message}");
            Environment.Exit(1);
        }
        catch (MarkdownProcessingException exception)
        {
            Console.WriteLine($"Failed to process markdown files: {exception.Message}");
            Environment.Exit(1);
        }
    }

    static void ApplyDefaults(Options options)
    {
        if (options.TargetDirectory == null)
        {
            options.TargetDirectory = Environment.CurrentDirectory;
            if (!GitRepoDirectoryFinder.IsInGitRepository(options.TargetDirectory))
            {
                Console.WriteLine($"The current directory does no exist with a .git repository. Pass in a target directory instead. Current directory: {options.TargetDirectory}");
                Environment.Exit(1);
            }
        }
        else
        {
            options.TargetDirectory = Path.GetFullPath(options.TargetDirectory);
        }
    }
}