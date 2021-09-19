public static class Extensions
{
    public static string ToCurrentDirectory(this string relativePath)
    {
        return Assembly.GetExecutingAssembly().ToCurrentDirectory(relativePath);
    }

    public static string ToCurrentDirectory(this Assembly assembly, string relativePath)
    {
        var fullPath = new Uri(assembly.Location!).AbsolutePath;
        var directory = Path.GetDirectoryName(fullPath)!;
        return Path.Combine(directory, relativePath);
    }
}