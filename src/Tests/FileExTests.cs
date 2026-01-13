public class FileExTests
{
    [Fact]
    public void FixFileCapitalization_ReturnsActualCasing()
    {
        using var tempDir = new TempDirectory();
        var actualPath = Path.Combine(tempDir, "TestFile.txt");
        File.WriteAllText(actualPath, "test");

        var inputPath = Path.Combine(tempDir, "testfile.txt");

        var result = FileEx.FixFileCapitalization(inputPath);

        Assert.Equal(actualPath, result);
    }

    [Fact]
    public void FixFileCapitalization_WorksWhenCasingMatches()
    {
        using var tempFile = TempFile.Create();
        var result = FileEx.FixFileCapitalization(tempFile);

        Assert.Equal(tempFile, result);
    }
}
