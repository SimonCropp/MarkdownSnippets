using ObjectApproval;

public class TestBase
{
    static TestBase()
    {
        SerializerBuilder.ExtraSettings = settings =>
        {
            var converters = settings.Converters;
            converters.Add(new ProcessResultConverter());
            converters.Add(new SnippetConverter());
        };
    }
}