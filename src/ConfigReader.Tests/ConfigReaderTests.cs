using System.IO;
using System.Text;
using ObjectApproval;
using Xunit;
using Xunit.Abstractions;

public class ConfigReaderTests :
    XunitLoggingBase
{
    [Fact]
    public void Empty()
    {
        var config = ReadConfig("{}");

        ObjectApprover.VerifyWithJson(config);
    }

    static Config ReadConfig(string input)
    {
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(input)))
        {
            return ConfigReader.Parse(stream);
        }
    }

    [Fact]
    public void Values()
    {
        var text = File.ReadAllText("sampleConfig.json");
        var config = ReadConfig(text);
        ObjectApprover.VerifyWithJson(config);
    }

    public ConfigReaderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}