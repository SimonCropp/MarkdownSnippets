using System.IO;
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
        var stream = File.ReadAllText("sampleConfig.json");
        var config = ConfigReader.Parse(stream);
        ObjectApprover.VerifyWithJson(config);
    }

    public ConfigReaderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}