public static class Scrubber
{
    public static string Scrub(string s)
    {
        var testDirectory = AssemblyLocation.CurrentDirectory;
        return s.Replace(@"\\", @"\")
            .ReplaceCaseless(testDirectory, @"root\");
    }
}