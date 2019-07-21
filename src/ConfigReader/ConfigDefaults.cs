using System;
using MarkdownSnippets;

public static class ConfigDefaults
{
    public static (bool readOnly, bool writeHeader, LinkFormat linkFormat) Convert(Config config, bool? inputReadOnly, bool? writeHeader, LinkFormat? linkFormat)
    {
        if (config == null)
        {
            return (
                readOnly: inputReadOnly.GetValueOrDefault(),
                writeHeader: writeHeader.GetValueOrDefault(true),
                linkFormat: linkFormat.GetValueOrDefault(LinkFormat.GitHub)
            );
        }

        var configLinkFormat = GetLinkFormat(config.LinkFormat);

        return (
            readOnly: inputReadOnly.GetValueOrDefault(config.ReadOnly.GetValueOrDefault(false)),
            writeHeader: writeHeader.GetValueOrDefault(config.WriteHeader.GetValueOrDefault(true)),
            linkFormat: linkFormat.GetValueOrDefault(configLinkFormat.GetValueOrDefault(LinkFormat.GitHub))
        );
    }

    static LinkFormat? GetLinkFormat(string value)
    {
        if (value == null)
        {
            return null;
        }

        if (!Enum.TryParse<LinkFormat>(value, out var linkFormat))
        {
            throw new Exception("Failed to parse LinkFormat:" + linkFormat);
        }

        return linkFormat;
    }
}