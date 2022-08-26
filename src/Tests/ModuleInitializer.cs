public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyDiffPlex.Initialize();
        VerifierSettings.IgnoreStackTrace();
        VerifierSettings.AddExtraSettings(serializerSettings =>
            {
                var converters = serializerSettings.Converters;
                converters.Add(new ProcessResultConverter());
                converters.Add(new SnippetConverter());
        });
    }
}