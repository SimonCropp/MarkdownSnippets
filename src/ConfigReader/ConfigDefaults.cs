using System.Collections.Generic;
using System.Linq;
using MarkdownSnippets;

public static class ConfigDefaults
{
    public static ConfigResult Convert(ConfigInput fileConfig, ConfigInput otherConfig)
    {
        if (fileConfig == null)
        {
            return new ConfigResult
            {
                ReadOnly = otherConfig.ReadOnly.GetValueOrDefault(),
                WriteHeader = otherConfig.WriteHeader.GetValueOrDefault(true),
                LinkFormat = otherConfig.LinkFormat.GetValueOrDefault(LinkFormat.GitHub),
                Exclude = otherConfig.Exclude,
                TocExcludes = otherConfig.TocExcludes,
                UrlsAsSnippets = otherConfig.UrlsAsSnippets,
                TocLevel = otherConfig.TocLevel.GetValueOrDefault(2)
            };
        }

        return new ConfigResult
        {
            ReadOnly = GetValueOrDefault("ReadOnly", otherConfig.ReadOnly, fileConfig.ReadOnly, false),
            WriteHeader = GetValueOrDefault("WriteHeader", otherConfig.WriteHeader, fileConfig.WriteHeader, true),
            LinkFormat = GetValueOrDefault("LinkFormat", otherConfig.LinkFormat, fileConfig.LinkFormat, LinkFormat.GitHub),
            TocLevel = GetValueOrDefault("TocLevel", otherConfig.TocLevel, fileConfig.TocLevel, 2),
            Exclude = JoinLists(fileConfig.Exclude, otherConfig.Exclude),
            TocExcludes = JoinLists(fileConfig.TocExcludes, otherConfig.TocExcludes),
            UrlsAsSnippets = JoinLists(fileConfig.UrlsAsSnippets, otherConfig.UrlsAsSnippets)
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
}