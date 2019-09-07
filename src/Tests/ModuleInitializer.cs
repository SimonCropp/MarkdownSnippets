using ObjectApproval;

static class ModuleInitializer
{
    public static void Initialize()
    {
        SerializerBuilder.ExtraSettings = settings =>
        {
            var converters = settings.Converters;
            converters.Add(new ProcessResultConverter());
            converters.Add(new SnippetConverter());
        };
    }
}