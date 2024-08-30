﻿var stopwatch = Stopwatch.StartNew();
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
    targetDirectory = Path.GetFullPath(targetDirectory);
    var (fileConfig, configFilePath) = ConfigReader.Read(targetDirectory);
    var configResult = ConfigDefaults.Convert(fileConfig, configInput);

    var message = LogBuilder.BuildConfigLogMessage(targetDirectory, configResult, configFilePath);
    Console.WriteLine(message);

    var processor = new DirectoryMarkdownProcessor(
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
        omitSnippetLinks: configResult.OmitSnippetLinks);

    var urlsAsSnippets = configResult.UrlsAsSnippets;
    if (urlsAsSnippets != null && urlsAsSnippets.Count != 0)
    {
        var snippets = new List<Snippet>();

        await snippets.AppendUrlsAsSnippets(urlsAsSnippets);
        processor.AddSnippets(snippets);
    }

    processor.Run();
}