using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownSnippets;

class Program
{
    static void Main(string[] args)
    {
        CommandRunner.RunCommand(Inner, args);
    }

    static void Inner(string targetDirectory, bool? commandLineReadOnly, bool? commandLineWriteHeader, List<string> excludes)
    {
        Console.WriteLine($"TargetDirectory: {targetDirectory}");
        if (!Directory.Exists(targetDirectory))
        {
            Console.WriteLine($"Target directory does not exist: {targetDirectory}");
            Environment.Exit(1);
        }

        if (excludes.Any())
        {
            var separator = Environment.NewLine + "\t";
            Console.WriteLine($"Excludes:{separator}{string.Join(separator, excludes)}");
        }

        excludes = GetExcludesWithBothSlashes(excludes).ToList();

        var config = ConfigReader.Read(targetDirectory);
        var (readOnly, writeHeader) = ConfigDefaults.Convert(config, commandLineReadOnly, commandLineWriteHeader);

        try
        {
            var processor = new DirectoryMarkdownProcessor(
                targetDirectory,
                log: Console.WriteLine,
                directoryFilter: path => !excludes.Any(path.Contains),
                readOnly: readOnly,
                writeHeader: writeHeader);
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

    static IEnumerable<string> GetExcludesWithBothSlashes(List<string> excludes)
    {
        foreach (var exclude in excludes)
        {
            yield return exclude.Replace('\\', '/');
            yield return exclude.Replace('/', '\\');
        }
    }
}