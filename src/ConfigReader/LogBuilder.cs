using System.IO;
using System.Linq;
using System.Text;

static class LogBuilder
{
    public static string BuildConfigLogMessage(string root, ConfigResult config, string configFilePath)
    {
        var builder = new StringBuilder($@"Config:
    RootDir: {root}
    ReadOnly: {config.ReadOnly}
    WriteHeader: {config.WriteHeader}
    LinkFormat: {config.LinkFormat}
    FileConfigPath: {configFilePath} (exists:{File.Exists(configFilePath)})
");

        if (config.Exclude.Any())
        {
            builder.AppendLine($@"    Exclude:
        {string.Join("        \r\n", config.Exclude)}");
        }

        if (config.UrlsAsSnippets.Any())
        {
            builder.AppendLine($@"    UrlsAsSnippets:
        {string.Join("        \r\n", config.UrlsAsSnippets)}");
        }

        return builder.ToString().Trim();
    }
}