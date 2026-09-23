public class IndexReaderTests
{
    [Test]
    [Arguments("a\r", "\r")]
    [Arguments("a\n", "\n")]
    [Arguments("a\r\n", "\r\n")]
    [Arguments("", null)]
    [Arguments("a", null)]
    [Arguments("a\rb", "\r")]
    [Arguments("a\nb", "\n")]
    [Arguments("a\r\nb", "\r\n")]
    [Arguments("a\r\r", "\r")]
    [Arguments("a\r\r\nb", "\r")]
    public async Task NewLineDetection(string input, string? expected)
    {
        var fileName = Path.GetTempFileName();
        try
        {
            File.WriteAllText(fileName, input);
            using var streamReader = File.OpenText(fileName);
            streamReader.TryFindNewline(out var newline);
            await Assert.That(newline).IsEqualTo(expected);
        }
        finally
        {
            File.Delete(fileName);
        }
    }
}