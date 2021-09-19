using Xunit;

public class IndexReaderTests
{
    [Theory]
    [InlineData("a\r", "\r")]
    [InlineData("a\n", "\n")]
    [InlineData("a\r\n", "\r\n")]
    [InlineData("", null)]
    [InlineData("a", null)]
    [InlineData("a\rb", "\r")]
    [InlineData("a\nb", "\n")]
    [InlineData("a\r\nb", "\r\n")]
    [InlineData("a\r\r", "\r")]
    [InlineData("a\r\r\nb", "\r")]
    public void NewLineDetection(string input, string? expected)
    {
        var fileName = Path.GetTempFileName();
        try
        {
            File.WriteAllText(fileName, input);
            using var streamReader = File.OpenText(fileName);
            streamReader.TryFindNewline(out var newline);
            Assert.Equal(expected, newline);
        }
        finally
        {
            File.Delete(fileName);
        }
    }
}