using System.IO;

static class AssemblyLocation
{
    static AssemblyLocation()
    {
        var assembly = typeof(AssemblyLocation).Assembly;

        var path = assembly.CodeBase
            .Replace("file:///", "")
            .Replace("file://", "")
            .Replace(@"file:\\\", "")
            .Replace(@"file:\\", "");

        CurrentDirectory = Path.GetDirectoryName(path);
    }

    public static string CurrentDirectory;
}