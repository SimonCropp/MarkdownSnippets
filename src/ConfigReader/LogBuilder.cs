using System;
using System.IO;
using System.Linq;
using System.Text;

static class LogBuilder
{
    public static string BuildConfigLogMessage(string root, ConfigResult config, string configFilePath)
    {
        var header = GetHeader(config);
        var builder = new StringBuilder($@"Config:
    RootDir: {root}
    ReadOnly: {config.ReadOnly}
    WriteHeader: {config.WriteHeader}
    Header: {header}
    UrlPrefix: {config.UrlPrefix}
    LinkFormat: {config.LinkFormat}
    Convention: {config.Convention}
    TocLevel: {config.TocLevel}
    MaxWidth: {config.MaxWidth}
    ValidateContent: {config.ValidateContent}
    FileConfigPath: {configFilePath} (exists:{File.Exists(configFilePath)})
");

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

        if (config.DocumentExtensions.Any())
        {
            builder.AppendLine($@"    DocumentExtensions:
        {string.Join("\r\n        ", config.DocumentExtensions)}");
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