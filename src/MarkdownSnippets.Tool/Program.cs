using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarkdownSnippets;

static class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            await CommandRunner.RunCommand(Inner, args);
        }
        catch (CommandLineException exception)
        {
            Console.WriteLine($"Failed: {exception.Message}");
            Environment.Exit(1);
        }
    }

    static async Task Inner(string targetDirectory, ConfigInput configInput)
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
            convention: configResult.Convention,
            tocExcludes: configResult.TocExcludes,
            documentExtensions: configResult.DocumentExtensions,
            tocLevel: configResult.TocLevel,
            treatMissingAsWarning: configResult.TreatMissingAsWarning,
            maxWidth: configResult.MaxWidth,
            validateContent: configResult.ValidateContent,
            hashSnippetAnchors: configResult.HashSnippetAnchors);

        var snippets = new List<Snippet>();
        await snippets.AppendUrlsAsSnippets(configResult.UrlsAsSnippets);
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