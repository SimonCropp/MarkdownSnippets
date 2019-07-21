public static class ConfigDefaults
{
    public static (bool readOnly, bool writeHeader) Convert(Config config, bool? inputReadOnly, bool? writeHeader)
    {
        if (config == null)
        {
            return (
                readOnly: inputReadOnly.GetValueOrDefault(),
                writeHeader: writeHeader.GetValueOrDefault(true)
            );
        }

        return (
            readOnly: inputReadOnly.GetValueOrDefault(config.ReadOnly.GetValueOrDefault(false)),
            writeHeader: writeHeader.GetValueOrDefault(config.WriteHeader.GetValueOrDefault(true))
        );
    }
}