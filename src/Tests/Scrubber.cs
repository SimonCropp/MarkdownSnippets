using System;
using System.IO;
using System.Text;
using MarkdownSnippets;

public static class Scrubber
{
    public static void Scrub(StringBuilder builder)
    {
        var gitDir = GitRepoDirectoryFinder.FindForFilePath().Replace('\\', '/');
        var testDirectory = AssemblyLocation.CurrentDirectory;
        builder.Replace(@"\\", @"\");
        builder.Replace(testDirectory, @"root\");
        builder.Replace(Environment.CurrentDirectory, "TheTargetDirectory");
        builder.Replace(Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../")), "TheTargetDirectory");
        builder.Replace("\\r\\n", "\r\n");
        builder.Replace(gitDir, "");
    }
}