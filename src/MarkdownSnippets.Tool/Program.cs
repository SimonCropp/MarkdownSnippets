using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MarkdownSnippets;

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
catch (ConfigurationException exception)
{
    Console.WriteLine($"Failed: {exception.Message}");
    Environment.Exit(1);
}
catch (MarkdownProcessingException exception)
{
    Console.WriteLine($"Failed: {exception.Message}, File:'{exception.File}', Line:{exception.LineNumber}.");
    Environment.Exit(1);
}
catch (SnippetException exception)
{
    Console.WriteLine($"Failed: {exception.Message}");
    Environment.Exit(1);
}

finally
{
    Console.WriteLine($"Finished {stopwatch.ElapsedMilliseconds}ms");
}

static async Task Inner(string targetDirectory, ConfigInput configInput)
{
    var (fileConfig, configFilePath) = ConfigReader.Read(targetDirectory);
    var configResult = ConfigDefaults.Convert(fileConfig, configInput);

    var message = LogBuilder.BuildConfigLogMessage(targetDirectory, configResult, configFilePath);
    Console.WriteLine(message);

    DirectoryMarkdownProcessor processor = new(
        targetDirectory,
        directoryIncludes: ExcludeToFilterBuilder.ExcludesToFilter(configResult.ExcludeDirectories),
        markdownDirectoryIncludes: ExcludeToFilterBuilder.ExcludesToFilter(configResult.ExcludeMarkdownDirectories),
        snippetDirectoryIncludes: ExcludeToFilterBuilder.ExcludesToFilter(configResult.ExcludeSnippetDirectories),
        convention: configResult.Convention,
        log: Console.WriteLine,
        writeHeader: configResult.WriteHeader,
        header: configResult.Header,
        readOnly: configResult.ReadOnly,
        linkFormat: configResult.LinkFormat,
        tocLevel: configResult.TocLevel,
        tocExcludes: configResult.TocExcludes,
        treatMissingAsWarning: configResult.TreatMissingAsWarning,
        maxWidth: configResult.MaxWidth,
        urlPrefix: configResult.UrlPrefix,
        validateContent: configResult.ValidateContent,
        hashSnippetAnchors: configResult.HashSnippetAnchors);

    if (configResult.UrlsAsSnippets.Any())
    {
        List<Snippet> snippets = new();

        await snippets.AppendUrlsAsSnippets(configResult.UrlsAsSnippets);
        processor.AddSnippets(snippets);
    }

    processor.Run();
}