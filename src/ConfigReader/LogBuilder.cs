using MarkdownSnippets;

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
            Polyfill.AppendLine(
                builder,
                $"""
                     ReadOnly: {config.ReadOnly}
                     WriteHeader: {config.WriteHeader}
                 """);
            var header1 = config.Header;
            if (header1 != null)
            {
                var newlineIndent = $"{Environment.NewLine}        ";
                header1 = string.Join(newlineIndent, header1.Lines());
                header1 = header1.Replace(@"\n", newlineIndent);
                var header = $"""

                                      {header1}
                              """;
                Polyfill.AppendLine(builder, $"    Header: {header}");
            }
        }

        var maxWidth = config.MaxWidth;
        if (maxWidth != int.MaxValue && maxWidth != 0)
        {
            Polyfill.AppendLine(
                builder,
                $"    MaxWidth: {maxWidth}");
        }

        AppendValues(config.ExcludeDirectories, builder, "ExcludeDirectories");
        AppendValues(config.ExcludeMarkdownDirectories, builder, "ExcludeMarkdownDirectories");
        AppendValues(config.ExcludeSnippetDirectories, builder, "ExcludeSnippetDirectories");
        AppendValues(config.TocExcludes, builder, "TocExcludes");
        AppendValues(config.UrlsAsSnippets, builder, "UrlsAsSnippets");

        return builder.ToString().Trim();
    }

    static void AppendValues(List<string> items, StringBuilder builder, string name)
    {
        if (items.Count == 0)
        {
            return;
        }

        Polyfill.AppendLine(builder, $"    {name}:");
        foreach (var item in items)
        {
            builder.Append("        ");
            builder.AppendLine(item);
        }
    }
}