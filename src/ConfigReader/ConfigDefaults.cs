using MarkdownSnippets;

public static class ConfigDefaults
{
    public static ConfigResult Convert(ConfigInput? fileConfig, ConfigInput otherConfig)
    {
        if (fileConfig == null)
        {
            return new()
            {
                ValidateContent = otherConfig.ValidateContent.GetValueOrDefault(),
                HashSnippetAnchors = otherConfig.HashSnippetAnchors.GetValueOrDefault(),
                OmitSnippetLinks = otherConfig.OmitSnippetLinks.GetValueOrDefault(),
                ReadOnly = otherConfig.ReadOnly.GetValueOrDefault(),
                WriteHeader = otherConfig.WriteHeader,
                LinkFormat = otherConfig.LinkFormat.GetValueOrDefault(LinkFormat.GitHub),
                Convention = otherConfig.Convention.GetValueOrDefault(DocumentConvention.SourceTransform),
                ExcludeDirectories = otherConfig.ExcludeDirectories,
                ExcludeMarkdownDirectories = otherConfig.ExcludeMarkdownDirectories,
                ExcludeSnippetDirectories = otherConfig.ExcludeSnippetDirectories,
                Header = otherConfig.Header,
                UrlPrefix = otherConfig.UrlPrefix,
                TocExcludes = otherConfig.TocExcludes,
                UrlsAsSnippets = otherConfig.UrlsAsSnippets,
                TocLevel = otherConfig.TocLevel.GetValueOrDefault(2),
                MaxWidth = otherConfig.MaxWidth.GetValueOrDefault(int.MaxValue),
                TreatMissingAsWarning = otherConfig.TreatMissingAsWarning.GetValueOrDefault(),
            };
        }

        return new()
        {
            ValidateContent = GetValueOrDefault("ValidateContent", otherConfig.ValidateContent, fileConfig.ValidateContent, false),
            HashSnippetAnchors = GetValueOrDefault("HashSnippetAnchors", otherConfig.HashSnippetAnchors, fileConfig.HashSnippetAnchors, false),
            OmitSnippetLinks = GetValueOrDefault("OmitSnippetLinks", otherConfig.OmitSnippetLinks, fileConfig.OmitSnippetLinks, false),
            ReadOnly = GetValueOrNull("ReadOnly", otherConfig.ReadOnly, fileConfig.ReadOnly),
            WriteHeader = GetValueOrNull("WriteHeader", otherConfig.WriteHeader, fileConfig.WriteHeader),
            LinkFormat = GetValueOrDefault("LinkFormat", otherConfig.LinkFormat, fileConfig.LinkFormat, LinkFormat.GitHub),
            Convention = GetValueOrDefault("Convention", otherConfig.Convention, fileConfig.Convention, DocumentConvention.SourceTransform),
            TocLevel = GetValueOrDefault("TocLevel", otherConfig.TocLevel, fileConfig.TocLevel, 2),
            MaxWidth = GetValueOrDefault("MaxWidth", otherConfig.MaxWidth, fileConfig.MaxWidth, int.MaxValue),
            Header = GetValueOrDefault("Header", otherConfig.Header, fileConfig.Header),
            UrlPrefix = GetValueOrDefault("UrlPrefix", otherConfig.UrlPrefix, fileConfig.UrlPrefix),
            ExcludeDirectories = JoinLists(fileConfig.ExcludeDirectories, otherConfig.ExcludeDirectories),
            ExcludeSnippetDirectories = JoinLists(fileConfig.ExcludeSnippetDirectories, otherConfig.ExcludeSnippetDirectories),
            ExcludeMarkdownDirectories = JoinLists(fileConfig.ExcludeMarkdownDirectories, otherConfig.ExcludeMarkdownDirectories),
            TocExcludes = JoinLists(fileConfig.TocExcludes, otherConfig.TocExcludes),
            UrlsAsSnippets = JoinLists(fileConfig.UrlsAsSnippets, otherConfig.UrlsAsSnippets),
            TreatMissingAsWarning = GetValueOrDefault(
                "TreatMissingAsWarning",
                otherConfig.TreatMissingAsWarning,
                fileConfig.TreatMissingAsWarning,
                false),
        };
    }

    static List<string> JoinLists(List<string> list1, List<string> list2) =>
        list1
            .Concat(list2)
            .Distinct()
            .ToList();

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

        return config;
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