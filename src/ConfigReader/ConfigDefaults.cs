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

        return (
            readOnly: inputReadOnly.GetValueOrDefault(config.ReadOnly.GetValueOrDefault(false)),
            writeHeader: writeHeader.GetValueOrDefault(config.WriteHeader.GetValueOrDefault(true)),
            linkFormat: linkFormat.GetValueOrDefault(config.LinkFormat.GetValueOrDefault(LinkFormat.GitHub))
        );
    }
}