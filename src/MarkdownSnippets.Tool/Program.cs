using System;
using System.Collections.Generic;
using MarkdownSnippets;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            CommandRunner.RunCommand(Inner, args);
        }
        catch (CommandLineException exception)
        {
            Console.WriteLine($"Failed: {exception.Message}");
            Environment.Exit(1);
        }
    }

    static void Inner(string targetDirectory, ConfigInput configInput)
    {
        var (fileConfig, configFilePath) = ConfigReader.Read(targetDirectory);
        var configResult = ConfigDefaults.Convert(fileConfig, configInput);

        var message = LogBuilder.BuildConfigLogMessage(targetDirectory, configResult, configFilePath);
        Console.WriteLine(message);

        var processor = new DirectoryMarkdownProcessor(
            targetDirectory,
            log: Console.WriteLine,
            directoryFilter: ExcludeToFilterBuilder.ExcludesToFilter(configResult.Exclude),
            readOnly: configResult.ReadOnly,
            writeHeader: configResult.WriteHeader,
            header: configResult.Header,
            urlPrefix: configResult.UrlPrefix,
            linkFormat: configResult.LinkFormat,
            tocExcludes: configResult.TocExcludes,
            tocLevel: configResult.TocLevel,
            treatMissingSnippetsAsWarnings: configResult.TreatMissingSnippetsAsWarnings,
            maxWidth: configResult.MaxWidth);

        var snippets = new List<Snippet>();
        snippets.AppendUrlsAsSnippets(configResult.UrlsAsSnippets).GetAwaiter().GetResult();
        processor.AddSnippets(snippets);

        try
        {
            processor.Run();
        }
        catch (SnippetException exception)
        {
            Console.WriteLine($"Failed: {exception.Message}");
            Environment.Exit(1);
        }
    }
}