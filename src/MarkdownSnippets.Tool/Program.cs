using System;
using System.IO;
using MarkdownSnippets;

class Program
{
    static void Main(string[] args)
    {
        GitHubMarkdownProcessor.Log = Console.WriteLine;
        var targetDirectory = GetTargetDirectory(args);
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

    static string GetTargetDirectory(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Only one argument (target directory) is supported");
            Environment.Exit(1);
        }

        if (args.Length == 1)
        {
            var targetDirectory = args[0];
            if (Directory.Exists(targetDirectory))
            {
                return targetDirectory;
            }
            Console.WriteLine($"Target directory does not exist: {targetDirectory}");
            Environment.Exit(1);
        }

        var currentDirectory = Environment.CurrentDirectory;
        if (!GitRepoDirectoryFinder.IsInGitRepository(currentDirectory))
        {
            Console.WriteLine($"The current directory does no exist with a .git repository. Pass in a target directory instead. Current directory: {currentDirectory}");
            Environment.Exit(1);
        }
        return currentDirectory;
    }
}