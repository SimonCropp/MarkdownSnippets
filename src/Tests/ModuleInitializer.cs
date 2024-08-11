public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyDiffPlex.Initialize(OutputType.Compact);
        VerifierSettings.IgnoreStackTrace();
        VerifierSettings.AddExtraSettings(serializer =>
        {
            var converters = serializer.Converters;
            converters.Add(new ProcessResultConverter());
            converters.Add(new SnippetConverter());
        });
        VerifierSettings.AddScrubber(_ => _.Replace('\\', '/'));
    }
}