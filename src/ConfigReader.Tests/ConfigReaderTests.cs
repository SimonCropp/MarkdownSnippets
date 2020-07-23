using System.IO;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class ConfigReaderTests
{
    [Fact]
    public Task Empty()
    {
        var config = ConfigReader.Parse("{}");

        return Verifier.Verify(config);
    }

    [Fact]
    public Task Values()
    {
        var stream = File.ReadAllText("sampleConfig.json");
        var config = ConfigReader.Parse(stream);
        return Verifier.Verify(config);
    }
}