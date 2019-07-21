using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class ConfigReaderTests :
    XunitLoggingBase
{
    [Fact]
    public void Empty()
    {
        var config = ConfigReader.Parse("{}");
        ObjectApprover.VerifyWithJson(config);
    }

    [Fact]
    public void Values()
    {
        var config = ConfigReader.Parse(@"
{
    ""WriteHeader"":true,
    ""ReadOnly"":false
}
");
        ObjectApprover.VerifyWithJson(config);
    }

    public ConfigReaderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}