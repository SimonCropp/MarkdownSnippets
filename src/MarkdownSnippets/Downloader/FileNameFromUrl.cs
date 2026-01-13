static class FileNameFromUrl
{
    static HashSet<char> invalid = [..Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars())];

    public static string ConvertToFileName(string url)
    {
        var builder = StringBuilderCache.Acquire(url.Length);
        foreach (var ch in url)
        {
            if (invalid.Contains(ch))
            {
                builder.Append('_');
                continue;
            }

            builder.Append(ch);
        }

        return StringBuilderCache.GetStringAndRelease(builder);
    }
}
