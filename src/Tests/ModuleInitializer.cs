using VerifyXunit;

static class ModuleInitializer
{
    public static void Initialize()
    {
        Global.ApplyExtraSettings(settings =>
        {
            var converters = settings.Converters;
            converters.Add(new ProcessResultConverter());
            converters.Add(new SnippetConverter());
        });
        Global.AddScrubber(Scrubber.Scrub);
    }
}