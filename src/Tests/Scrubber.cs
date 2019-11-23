using System;
using System.IO;
using MarkdownSnippets;

public static class Scrubber
{
    public static string Scrub(string s)
    {
        var gitDir = GitRepoDirectoryFinder.FindForFilePath().Replace('\\', '/');
        var testDirectory = AssemblyLocation.CurrentDirectory;
        return s.Replace(@"\\", @"\")
            .ReplaceCaseless(testDirectory, @"root\")
            .Replace(Environment.CurrentDirectory, "TheTargetDirectory")
            .Replace(Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../")), "TheTargetDirectory")
            .Replace("\\r\\n", "\r\n")
            .Replace(gitDir, "");
    }
}