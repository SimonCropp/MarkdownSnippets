using System.IO;

static class GitHashReader
{
    public static string GetHash(string directory)
    {
        var gitDirectory = Path.Combine(directory, ".git");
        return GetHashForGitDirectory(gitDirectory);
    }

    public static string GetHashForGitDirectory(string gitDirectory)
    {
        var headPath = Path.Combine(gitDirectory, "HEAD");
        var line = ReadFirstLine(headPath);
        if (!line.StartsWith("ref: "))
        {
            return line;
        }
        var head = line.Substring(5);
        var @ref = Path.Combine(gitDirectory, head);
        return ReadFirstLine(@ref);
    }

    static string ReadFirstLine(string head)
    {
        using (var stream = FileEx.OpenRead(head))
        using (var reader = new StreamReader(stream))
        {
            return reader.ReadLine();
        }
    }
}