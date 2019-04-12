using System.IO;
using Xunit;
using Xunit.Abstractions;

public class IndexReaderTests :
    TestBase
{
    [Theory]
    [InlineData("a\r", "\r")]
    [InlineData("a\r\n", "\r\n")]
    [InlineData("", "\r\n")]
    [InlineData("a", "\r\n")]
    [InlineData("a\rb", "\r")]
    [InlineData("a\r\nb", "\r\n")]
    [InlineData("a\r\r", "\r")]
    [InlineData("a\r\r\nb", "\r")]
    public void NewLineDetection(string input, string expected)
    {
        var fileName = Path.GetTempFileName();
        try
        {
            File.WriteAllText(fileName, input);
            using (var streamReader = File.OpenText(fileName))
            {
                var indexReader = new IndexReader(streamReader);
                indexReader.ReadLine();
                Assert.Equal(expected, indexReader.NewLine);
            }
        }
        finally
        {
            File.Delete(fileName);
        }
    }

    public IndexReaderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}