using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MarkdownSnippets;

static class Program
{
    static async Task Main(string[] args)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await CommandRunner.RunCommand(Inner, args);
        }
        catch (CommandLineException exception)
        {
            Console.WriteLine($"Failed: {exception.Message}");
            Environment.Exit(1);
        }
        finally
        {
            Console.WriteLine($"Finished {stopwatch.ElapsedMilliseconds}ms");
        }
    }

    static async Task Inner(string targetDirectory, ConfigInput configInput)
    {
        var (fileConfig, configFilePath) = ConfigReader.Read(targetDirectory);
        var configResult = ConfigDefaults.Convert(fileConfig, configInput);

        var message = LogBuilder.BuildConfigLogMessage(targetDirectory, configResult, configFilePath);
        Console.WriteLine(message);

        try
        {
            DirectoryMarkdownProcessor processor = new(
                targetDirectory,
                shouldIncludeDirectory: ExcludeToFilterBuilder.ExcludesToFilter(configResult.Exclude),
                convention: configResult.Convention,
                log: Console.WriteLine,
                writeHeader: configResult.WriteHeader,
                header: configResult.Header,
                readOnly: configResult.ReadOnly,
                linkFormat: configResult.LinkFormat,
                tocLevel: configResult.TocLevel,
                tocExcludes: configResult.TocExcludes,
                documentExtensions: configResult.DocumentExtensions,
                treatMissingAsWarning: configResult.TreatMissingAsWarning,
                maxWidth: configResult.MaxWidth,
                urlPrefix: configResult.UrlPrefix,
                validateContent: configResult.ValidateContent,
                hashSnippetAnchors: configResult.HashSnippetAnchors);

            List<Snippet> snippets = new();
            await snippets.AppendUrlsAsSnippets(configResult.UrlsAsSnippets);
            processor.AddSnippets(snippets);

            processor.Run();
        }
        catch (SnippetException exception)
        {
            Console.WriteLine($"Failed: {exception.Message}");
            Environment.Exit(1);
        }
    }
}