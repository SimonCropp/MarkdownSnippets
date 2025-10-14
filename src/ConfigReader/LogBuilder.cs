static class LogBuilder
{
    public static string BuildConfigLogMessage(string targetDirectory, ConfigResult config, string configFilePath)
    {
        var builder = new StringBuilder(
            $"""
             Config:
                 TargetDirectory: {targetDirectory}
                 UrlPrefix: {config.UrlPrefix}
                 LinkFormat: {config.LinkFormat}
                 Convention: {config.Convention}
                 TocLevel: {config.TocLevel}
                 ValidateContent: {config.ValidateContent}
                 OmitSnippetLinks: {config.OmitSnippetLinks}
                 TreatMissingAsWarning: {config.TreatMissingAsWarning}
                 FileConfigPath: {configFilePath} (exists:{File.Exists(configFilePath)})

             """);

        if (config.Convention == DocumentConvention.SourceTransform)
        {
            var header = GetHeader(config);
            Polyfill.AppendLine(
                builder,
                $"""
                     ReadOnly: {config.ReadOnly}
                     WriteHeader: {config.WriteHeader}
                     Header: {header}
                 """);
        }

        var maxWidth = config.MaxWidth;
        if (maxWidth != int.MaxValue && maxWidth != 0)
        {
            Polyfill.AppendLine(
                builder,
                $"    MaxWidth: {maxWidth}");
        }

        var excludeDirectories = config.ExcludeDirectories;
        if (excludeDirectories != null && excludeDirectories.Count != 0)
        {
            Polyfill.AppendLine(
                builder,
                $"""
                     ExcludeDirectories:
                         {string.Join("\r\n        ", excludeDirectories)}
                 """);
        }

        var excludeMarkdownDirectories = config.ExcludeMarkdownDirectories;
        if (excludeMarkdownDirectories != null && excludeMarkdownDirectories.Count != 0)
        {
            Polyfill.AppendLine(
                builder,
                $"""
                     ExcludeMarkdownDirectories:
                         {string.Join("\r\n        ", excludeMarkdownDirectories)}
                 """);
        }

        var excludeSnippetDirectories = config.ExcludeSnippetDirectories;
        if (excludeSnippetDirectories != null && excludeSnippetDirectories.Count != 0)
        {
            Polyfill.AppendLine(
                builder,
                $"""
                     ExcludeSnippetDirectories:
                         {string.Join("\r\n        ", excludeSnippetDirectories)}
                 """);
        }

        var tocExcludes = config.TocExcludes;
        if (tocExcludes != null && tocExcludes.Count != 0)
        {
            Polyfill.AppendLine(
                builder,
                $"""
                     TocExcludes:
                         {string.Join("\r\n        ", tocExcludes)}
                 """);
        }

        var urlsAsSnippets = config.UrlsAsSnippets;
        if (urlsAsSnippets != null && urlsAsSnippets.Count != 0)
        {
            Polyfill.AppendLine(
                builder,
                $"""
                     UrlsAsSnippets:
                         {string.Join("\r\n        ", urlsAsSnippets)}
                 """);
        }

#if NET48
        builder.AppendLine("    TargetFramework: net48");
#endif
#if NET9_0
        builder.AppendLine("    TargetFramework: net9.0");
#endif
        return builder.ToString().Trim();
    }

    static string? GetHeader(ConfigResult config)
    {
        var header = config.Header;
        if (header == null)
        {
            return null;
        }

        var newlineIndent = $"{Environment.NewLine}        ";
        header = string.Join(newlineIndent, header.Lines());
        header = header.Replace(@"\n", newlineIndent);
        return $"""

                        {header}
                """;
    }
}