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

    static void Inner(string targetDirectory, bool? commandLineReadOnly, bool? commandLineWriteHeader, LinkFormat? commandLineLinkFormat, List<string> excludes)
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

        var fileConfig = ConfigReader.Read(targetDirectory);
        var configResult = ConfigDefaults.Convert(
            fileConfig,
            new ConfigInput
            {
                ReadOnly = commandLineReadOnly,
                WriteHeader = commandLineWriteHeader,
                LinkFormat = commandLineLinkFormat
            });

        try
        {
            var processor = new DirectoryMarkdownProcessor(
                targetDirectory,
                log: Console.WriteLine,
                directoryFilter: path => !excludes.Any(path.Contains),
                readOnly: configResult.ReadOnly,
                writeHeader: configResult.WriteHeader,
                linkFormat: configResult.LinkFormat);
            processor.Run();
        }
        catch (SnippetException exception)
        {
            Console.WriteLine($"Failed: {exception.Message}");
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