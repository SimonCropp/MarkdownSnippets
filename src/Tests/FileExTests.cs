public class FileExTests
{
    [Test]
    public async Task MakeReadOnly_SetsReadOnlyAttribute()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            FileEx.MakeReadOnly(tempFile);

            var attributes = File.GetAttributes(tempFile);
            await Assert.That((attributes & FileAttributes.ReadOnly) != 0).IsTrue();
        }
        finally
        {
            File.SetAttributes(tempFile, FileAttributes.Normal);
            File.Delete(tempFile);
        }
    }

    [Test]
    public async Task ClearReadOnly_RemovesReadOnlyAttribute()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.SetAttributes(tempFile, File.GetAttributes(tempFile) | FileAttributes.ReadOnly);

            FileEx.ClearReadOnly(tempFile);

            var attributes = File.GetAttributes(tempFile);
            await Assert.That((attributes & FileAttributes.ReadOnly) != 0).IsFalse();
        }
        finally
        {
            File.SetAttributes(tempFile, FileAttributes.Normal);
            File.Delete(tempFile);
        }
    }

    [Test]
    public async Task ClearReadOnly_DoesNothingIfFileDoesNotExist()
    {
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        FileEx.ClearReadOnly(nonExistentFile);

        await Assert.That(File.Exists(nonExistentFile)).IsFalse();
    }

    [Test]
    public async Task MakeReadOnly_ThenClearReadOnly_RoundTrip()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            FileEx.MakeReadOnly(tempFile);
            await Assert.That((File.GetAttributes(tempFile) & FileAttributes.ReadOnly) != 0).IsTrue();

            FileEx.ClearReadOnly(tempFile);
            await Assert.That((File.GetAttributes(tempFile) & FileAttributes.ReadOnly) != 0).IsFalse();
        }
        finally
        {
            File.SetAttributes(tempFile, FileAttributes.Normal);
            File.Delete(tempFile);
        }
    }

    [Test]
    public async Task FixFileCapitalization_ReturnsActualCasing()
    {
        using var tempDir = new TempDirectory();
        var actualPath = Path.Combine(tempDir, "TestFile.txt");
        File.WriteAllText(actualPath, "test");

        var inputPath = Path.Combine(tempDir, "testfile.txt");

        var result = FileEx.FixFileCapitalization(inputPath);

        await Assert.That(result).IsEqualTo(actualPath);
    }

    [Test]
    public async Task FixFileCapitalization_WorksWhenCasingMatches()
    {
        using var tempFile = TempFile.Create();
        var result = FileEx.FixFileCapitalization(tempFile);

        await Assert.That(result).IsEqualTo(tempFile);
    }
}
