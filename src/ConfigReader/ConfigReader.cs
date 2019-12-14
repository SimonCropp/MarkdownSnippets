using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
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

        return (Parse(File.ReadAllText(path)), path);
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

    public static ConfigInput Parse(string contents)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents))
        {
            Position = 0
        };
        var serializer = new DataContractJsonSerializer(typeof(ConfigSerialization));
        var configSerialization = (ConfigSerialization) serializer.ReadObject(stream);
        return new ConfigInput
        {
            WriteHeader = configSerialization.WriteHeader,
            ReadOnly = configSerialization.ReadOnly,
            UrlsAsSnippets = configSerialization.UrlsAsSnippets,
            Exclude = configSerialization.Exclude,
            Header = configSerialization.Header,
            TocExcludes = configSerialization.TocExcludes,
            TocLevel = configSerialization.TocLevel,
            LinkFormat = GetLinkFormat(configSerialization.LinkFormat),
            TreatMissingSnippetsAsWarnings = configSerialization.TreatMissingSnippetsAsWarnings
        };
    }

    static LinkFormat? GetLinkFormat(string? value)
    {
        if (value == null)
        {
            return null;
        }

        if (!Enum.TryParse<LinkFormat>(value, out var linkFormat))
        {
            throw new ConfigurationException("Failed to parse LinkFormat:" + linkFormat);
        }

        return linkFormat;
    }
}