﻿using System;
using System.IO;
using System.Runtime.Serialization;
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
        var config = DeSerialize(contents);
        return new()
        {
            WriteHeader = config.WriteHeader,
            ReadOnly = config.ReadOnly,
            ValidateContent = config.ValidateContent,
            HashSnippetAnchors = config.HashSnippetAnchors,
            UrlsAsSnippets = config.UrlsAsSnippets,
            Exclude = config.Exclude,
            Header = config.Header,
            UrlPrefix = config.UrlPrefix,
            TocExcludes = config.TocExcludes,
            TocLevel = config.TocLevel,
            MaxWidth = config.MaxWidth,
            LinkFormat = GetLinkFormat(config.LinkFormat),
            Convention = GetConvention(config.Convention),
            TreatMissingAsWarning = config.TreatMissingAsWarning,
        };
    }

    static ConfigSerialization DeSerialize(string contents)
    {
        using MemoryStream stream = new(Encoding.UTF8.GetBytes(contents))
        {
            Position = 0
        };
        DataContractJsonSerializer serializer = new(typeof(ConfigSerialization));
        try
        {
            return (ConfigSerialization) serializer.ReadObject(stream)!;
        }
        catch (SerializationException exception)
        {
            throw new SnippetException($@"Failed to deserialize configuration. Error: {exception.Message}.
Content:
{contents}");
        }
    }

    static DocumentConvention? GetConvention(string? value)
    {
        if (value == null)
        {
            return null;
        }

        if (Enum.TryParse<DocumentConvention>(value, out var convention))
        {
            return convention;
        }

        throw new ConfigurationException($"Failed to parse DocumentConvention: {convention}");
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