namespace MarkdownSnippets;

public static class DefaultDirectoryExclusions
{
    public static bool ShouldExcludeDirectory(CharSpan path)
    {
        
#if NETSTANDARD
        var suffix = Path.GetFileName(path.ToString()).AsSpan();
        #else
        var suffix = Path.GetFileName(path);
        #endif

        if (suffix.StartsWith('.'))
        {
            return true;
        }

        if (suffix.SequenceEqual("packages".AsSpan()))
        {
            return true;
        }

        if (suffix.SequenceEqual("node_modules".AsSpan()))
        {
            return true;
        }

        if (suffix.SequenceEqual("bin".AsSpan()))
        {
            return true;
        }

        return suffix.SequenceEqual("obj".AsSpan());
    }
}