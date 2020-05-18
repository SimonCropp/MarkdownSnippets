using Verify;
using Xunit;

[GlobalSetUp]
public static class GlobalSetup
{
    public static void Setup()
    {
        SharedVerifySettings.DisableNewLineEscaping();
        SharedVerifySettings.ModifySerialization(settings =>
        {
            settings.AddExtraSettings(serializerSettings =>
            {
                var converters = serializerSettings.Converters;
                converters.Add(new ProcessResultConverter());
                converters.Add(new SnippetConverter());
            });
        });
        SharedVerifySettings.AddScrubber(Scrubber.Scrub);
    }
}