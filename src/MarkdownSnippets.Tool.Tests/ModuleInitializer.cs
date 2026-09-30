public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.IgnoreStackTrace();
        VerifierSettings.AddScrubber(_ => _.Replace('\\', '/'));
    }
}