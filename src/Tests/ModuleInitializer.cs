public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.IgnoreStackTrack();
        VerifierSettings.AddExtraSettings(serializerSettings =>
            {
                var converters = serializerSettings.Converters;
                converters.Add(new ProcessResultConverter());
                converters.Add(new SnippetConverter());
        });
    }
}