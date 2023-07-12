using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using MarkdownSnippets;

public static class ConfigReader
{
    public static (ConfigInput? config, string path) Read(string directory)
    {
        var exists = TryFindConfigFile(directory, out var path);
        if (!exists)
        {
            return (null, path);
        }

        return (Parse(File.ReadAllText(path), path), path);
    }

    static bool TryFindConfigFile(string directory, out string path)
    {
        path = Path.Combine(directory, "mdsnippets.json");
        var exists = File.Exists(path);
        if (exists)
        {
            return true;
        }

        foreach (var subDirectory in Directory.EnumerateDirectories(directory))
        {
            path = Path.Combine(subDirectory, "mdsnippets.json");
            exists = File.Exists(path);
            if (exists)
            {
                return true;
            }
        }

        path = "";
        return false;
    }

    public static ConfigInput Parse(string contents, string path)
    {
        var config = DeSerialize(contents);

        return new()
        {
            WriteHeader = config.WriteHeader,
            ReadOnly = config.ReadOnly,
            ValidateContent = config.ValidateContent,
            OmitSnippetLinks = config.OmitSnippetLinks,
            UrlsAsSnippets = config.UrlsAsSnippets,
            ExcludeDirectories = config.ExcludeDirectories,
            ExcludeMarkdownDirectories = config.ExcludeMarkdownDirectories,
            ExcludeSnippetDirectories = config.ExcludeSnippetDirectories,
            Header = config.Header,
            UrlPrefix = config.UrlPrefix,
            TocExcludes = config.TocExcludes,
            TocLevel = config.TocLevel,
            MaxWidth = config.MaxWidth,
            LinkFormat = GetLinkFormat(config.LinkFormat, path),
            Convention = GetConvention(config.Convention, path),
            TreatMissingAsWarning = config.TreatMissingAsWarning,
        };
    }

    static ConfigSerialization DeSerialize(string contents)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents))
        {
            Position = 0
        };
        var serializer = new DataContractJsonSerializer(typeof(ConfigSerialization));
        try
        {
            return (ConfigSerialization) serializer.ReadObject(stream)!;
        }
        catch (SerializationException exception)
        {
            throw new SnippetException($"""
                                        Failed to deserialize configuration. Error: {exception.Message}.
                                        Content:
                                        {contents}
                                        """);
        }
    }

    static DocumentConvention? GetConvention(string? value, string path)
    {
        if (value == null)
        {
            return null;
        }

        if (Enum.TryParse<DocumentConvention>(value, out var convention))
        {
            return convention;
        }

        throw new ConfigurationException($"Failed to parse DocumentConvention: {convention}. FilePath: {path}.");
    }

    static LinkFormat? GetLinkFormat(string? value, string path)
    {
        if (value == null)
        {
            return null;
        }

        if (!Enum.TryParse<LinkFormat>(value, out var linkFormat))
        {
            throw new ConfigurationException($"Failed to parse LinkFormat: {linkFormat}. FilePath: {path}.");
        }

        return linkFormat;
    }
}