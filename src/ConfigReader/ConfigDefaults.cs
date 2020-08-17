using System.Collections.Generic;
using System.Linq;
using MarkdownSnippets;

public static class ConfigDefaults
{
    public static ConfigResult Convert(ConfigInput? fileConfig, ConfigInput otherConfig)
    {
        if (fileConfig == null)
        {
            return new ConfigResult
            {
                ValidateContent = otherConfig.ValidateContent.GetValueOrDefault(),
                ReadOnly = otherConfig.ReadOnly.GetValueOrDefault(),
                WriteHeader = otherConfig.WriteHeader,
                LinkFormat = otherConfig.LinkFormat.GetValueOrDefault(LinkFormat.GitHub),
                Convention = otherConfig.Convention.GetValueOrDefault(DocumentConvention.SourceTransform),
                Exclude = otherConfig.Exclude,
                Header = otherConfig.Header,
                UrlPrefix = otherConfig.UrlPrefix,
                TocExcludes = otherConfig.TocExcludes,
                UrlsAsSnippets = otherConfig.UrlsAsSnippets,
                DocumentExtensions = otherConfig.DocumentExtensions,
                TocLevel = otherConfig.TocLevel.GetValueOrDefault(2),
                MaxWidth = otherConfig.MaxWidth.GetValueOrDefault(int.MaxValue),
                TreatMissingAsWarning = otherConfig.TreatMissingAsWarning.GetValueOrDefault()
            };
        }

        return new ConfigResult
        {
            ValidateContent = GetValueOrDefault("ValidateContent", otherConfig.ValidateContent, fileConfig.ValidateContent, false),
            ReadOnly = GetValueOrNull("ReadOnly", otherConfig.ReadOnly, fileConfig.ReadOnly),
            WriteHeader = GetValueOrNull("WriteHeader", otherConfig.WriteHeader, fileConfig.WriteHeader),
            LinkFormat = GetValueOrDefault("LinkFormat", otherConfig.LinkFormat, fileConfig.LinkFormat, LinkFormat.GitHub),
            Convention = GetValueOrDefault("Mode", otherConfig.Convention, fileConfig.Convention, DocumentConvention.SourceTransform),
            TocLevel = GetValueOrDefault("TocLevel", otherConfig.TocLevel, fileConfig.TocLevel, 2),
            MaxWidth = GetValueOrDefault("MaxWidth", otherConfig.MaxWidth, fileConfig.MaxWidth, int.MaxValue),
            Header = GetValueOrDefault("Header", otherConfig.Header, fileConfig.Header),
            UrlPrefix = GetValueOrDefault("UrlPrefix", otherConfig.UrlPrefix, fileConfig.UrlPrefix),
            Exclude = JoinLists(fileConfig.Exclude, otherConfig.Exclude),
            TocExcludes = JoinLists(fileConfig.TocExcludes, otherConfig.TocExcludes),
            UrlsAsSnippets = JoinLists(fileConfig.UrlsAsSnippets, otherConfig.UrlsAsSnippets),
            TreatMissingAsWarning = GetValueOrDefault(
                "TreatMissingAsWarning",
                otherConfig.TreatMissingAsWarning,
                fileConfig.TreatMissingAsWarning,
                false),
            DocumentExtensions = JoinLists(fileConfig.DocumentExtensions, otherConfig.DocumentExtensions)
        };
    }

    static List<string> JoinLists(List<string> list1, List<string> list2)
    {
        return list1
            .Concat(list2)
            .Distinct()
            .ToList();
    }

    static T GetValueOrDefault<T>(string name, T? input, T? config, T defaultValue)
        where T : struct
    {
        if (input != null && config != null)
        {
            throw new ConfigurationException($"'{name}' cannot be defined in both mdsnippets.json and input");
        }

        if (input != null)
        {
            return input.Value;
        }

        if (config != null)
        {
            return config.Value;
        }

        return defaultValue;
    }

    static T? GetValueOrNull<T>(string name, T? input, T? config)
        where T : struct
    {
        if (input != null &&
            config != null)
        {
            throw new ConfigurationException($"'{name}' cannot be defined in both mdsnippets.json and input");
        }

        if (input != null)
        {
            return input.Value;
        }

        if (config != null)
        {
            return config.Value;
        }

        return null;
    }

    static string? GetValueOrDefault(string name, string? input, string? config)
    {
        if (input != null && config != null)
        {
            throw new ConfigurationException($"'{name}' cannot be defined in both mdsnippets.json and input");
        }

        if (input != null)
        {
            return input;
        }

        return config;
    }
}