using System.IO;

static class DirectoryDateFinder
{
    public static long GetLastDirectoryWrite(string rootDirectory)
    {
        var lastHigh = File.GetLastWriteTime(rootDirectory);
        foreach (var directory in Directory.EnumerateDirectories(rootDirectory, "*", SearchOption.AllDirectories))
        {
            var lastWriteTime = File.GetLastWriteTime(directory);
            if (lastWriteTime > lastHigh)
            {
                lastHigh = lastWriteTime;
            }
        }

        return lastHigh.Ticks;
    }
}