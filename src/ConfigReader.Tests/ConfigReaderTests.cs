using System.IO;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ConfigReaderTests :
    VerifyBase
{
    [Fact]
    public Task Empty()
    {
        var config = ConfigReader.Parse("{}");

        return Verify(config);
    }

    [Fact]
    public Task Values()
    {
        var stream = File.ReadAllText("sampleConfig.json");
        var config = ConfigReader.Parse(stream);
        return Verify(config);
    }

    public ConfigReaderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}