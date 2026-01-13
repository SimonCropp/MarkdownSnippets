public class FileExTests
{
    [Fact]
    public void MakeReadOnly_SetsReadOnlyAttribute()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            FileEx.MakeReadOnly(tempFile);

            var attributes = File.GetAttributes(tempFile);
            Assert.True((attributes & FileAttributes.ReadOnly) != 0);
        }
        finally
        {
            File.SetAttributes(tempFile, FileAttributes.Normal);
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void ClearReadOnly_RemovesReadOnlyAttribute()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.SetAttributes(tempFile, File.GetAttributes(tempFile) | FileAttributes.ReadOnly);

            FileEx.ClearReadOnly(tempFile);

            var attributes = File.GetAttributes(tempFile);
            Assert.False((attributes & FileAttributes.ReadOnly) != 0);
        }
        finally
        {
            File.SetAttributes(tempFile, FileAttributes.Normal);
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void ClearReadOnly_DoesNothingIfFileDoesNotExist()
    {
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        FileEx.ClearReadOnly(nonExistentFile);

        Assert.False(File.Exists(nonExistentFile));
    }

    [Fact]
    public void MakeReadOnly_ThenClearReadOnly_RoundTrip()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            FileEx.MakeReadOnly(tempFile);
            Assert.True((File.GetAttributes(tempFile) & FileAttributes.ReadOnly) != 0);

            FileEx.ClearReadOnly(tempFile);
            Assert.False((File.GetAttributes(tempFile) & FileAttributes.ReadOnly) != 0);
        }
        finally
        {
            File.SetAttributes(tempFile, FileAttributes.Normal);
            File.Delete(tempFile);
        }
    }
}
