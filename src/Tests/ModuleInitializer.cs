using System;
using System.Runtime.CompilerServices;
using VerifyTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.ModifySerialization(settings =>
        {
            settings.AddExtraSettings(serializerSettings =>
            {
                var converters = serializerSettings.Converters;
                converters.Add(new ProcessResultConverter());
                converters.Add(new SnippetConverter());
            });
        });
        VerifierSettings.ModifySerialization(
            settings => settings.IgnoreMember<Exception>(
                exception => exception.StackTrace));
    }
}