using System;
using System.IO;

public static class Scrubber
{
    public static string Scrub(string s)
    {
        var testDirectory = AssemblyLocation.CurrentDirectory;
        return s.Replace(@"\\", @"\")
            .ReplaceCaseless(testDirectory, @"root\")
            .Replace(Environment.CurrentDirectory, "TheTargetDirectory")
            .Replace(Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../")), "TheTargetDirectory");
    }
}