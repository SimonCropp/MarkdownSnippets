using System.IO;
using Xunit;

public class IndexReaderTests
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
            using var streamReader = File.OpenText(fileName);
            var lines = LineReader.ReadFirstLine(streamReader);
            Assert.Equal(expected, lines.newLine);
        }
        finally
        {
            File.Delete(fileName);
        }
    }
}