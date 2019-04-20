using System;
using System.IO;
using MarkdownSnippets;

class Program
{
    static void Main(string[] args)
    {
        CommandRunner.RunCommand(args, Inner);
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
            var processor = new DirectoryMarkdownProcessor(targetDirectory, log: Console.WriteLine);
            processor.Run();
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
}