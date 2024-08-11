static class LogBuilder
{
    public static string BuildConfigLogMessage(string targetDirectory, ConfigResult config, string configFilePath)
    {
        var header = GetHeader(config);
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

        if (config.ExcludeDirectories.Count != 0)
        {
            Polyfill.AppendLine(
                builder,
                $"""
                     ExcludeDirectories:
                         {string.Join("\r\n        ", config.ExcludeDirectories)}
                 """);
        }

        if (config.ExcludeMarkdownDirectories.Count != 0)
        {
            Polyfill.AppendLine(
                builder,
                $"""
                     ExcludeMarkdownDirectories:
                         {string.Join("\r\n        ", config.ExcludeMarkdownDirectories)}
                 """);
        }

        if (config.ExcludeSnippetDirectories.Count != 0)
        {
            Polyfill.AppendLine(
                builder,
                $"""
                     ExcludeSnippetDirectories:
                         {string.Join("\r\n        ", config.ExcludeSnippetDirectories)}
                 """);
        }

        if (config.TocExcludes.Count != 0)
        {
            Polyfill.AppendLine(
                builder,
                $"""
                     TocExcludes:
                         {string.Join("\r\n        ", config.TocExcludes)}
                 """);
        }

        if (config.UrlsAsSnippets.Count != 0)
        {
            Polyfill.AppendLine(
                builder,
                $"""
                     UrlsAsSnippets:
                         {string.Join("\r\n        ", config.UrlsAsSnippets)}
                 """);
        }

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