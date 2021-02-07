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
        var config = ConfigReader.Parse("{}", "filePath");

        return Verifier.Verify(config);
    }

    [Fact]
    public Task BadJson()
    {
        return Verifier.Throws(
            () => ConfigReader.Parse(@"{
  ""ValidateContent"": true
  ""Convention"": ""InPlaceOverwrite""
}", "filePath"));
    }

    [Fact]
    public Task Values()
    {
        var stream = File.ReadAllText("allConfig.json");
        var config = ConfigReader.Parse(stream, "filePath");
        return Verifier.Verify(config);
    }
}