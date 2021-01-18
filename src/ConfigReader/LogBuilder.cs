using System;
using System.IO;
using System.Linq;
using System.Text;
using MarkdownSnippets;

static class LogBuilder
{
    public static string BuildConfigLogMessage(string targetDirectory, ConfigResult config, string configFilePath)
    {
        var header = GetHeader(config);
        StringBuilder builder = new($@"Config:
    TargetDirectory: {targetDirectory}
    UrlPrefix: {config.UrlPrefix}
    LinkFormat: {config.LinkFormat}
    Convention: {config.Convention}
    TocLevel: {config.TocLevel}
    ValidateContent: {config.ValidateContent}
    HashSnippetAnchors: {config.HashSnippetAnchors}
    TreatMissingAsWarning: {config.TreatMissingAsWarning}
    FileConfigPath: {configFilePath} (exists:{File.Exists(configFilePath)})
");

        if (config.Convention == DocumentConvention.SourceTransform)
        {
            builder.AppendLine(@$"    ReadOnly: {config.ReadOnly}
    WriteHeader: {config.WriteHeader}
    Header: {header}");
        }

        var maxWidth = config.MaxWidth;
        if (maxWidth != int.MaxValue && maxWidth != 0)
        {
            builder.AppendLine(@$"    MaxWidth: {maxWidth}");
        }

        if (config.Exclude.Any())
        {
            builder.AppendLine($@"    Exclude:
        {string.Join("\r\n        ", config.Exclude)}");
        }

        if (config.TocExcludes.Any())
        {
            builder.AppendLine($@"    TocExcludes:
        {string.Join("\r\n        ", config.TocExcludes)}");
        }

        if (config.UrlsAsSnippets.Any())
        {
            builder.AppendLine($@"    UrlsAsSnippets:
        {string.Join("\r\n        ", config.UrlsAsSnippets)}");
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
        return $@"
        {header}";
    }
}