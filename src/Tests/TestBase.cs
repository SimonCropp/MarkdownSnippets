using ObjectApproval;
using Xunit.Abstractions;

public class TestBase:
    XunitLoggingBase
{
    public TestBase(ITestOutputHelper output) :
        base(output)
    {
    }

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