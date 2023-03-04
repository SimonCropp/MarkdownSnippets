static class PathEx
{
#if NETSTANDARD

    static bool IsDirectorySeparator(char c) =>
        c is '/' or '\\';

    public static ReadOnlySpan<char> GetExtension(ReadOnlySpan<char> path)
    {
        int length = path.Length;

        for (int i = length - 1; i >= 0; i--)
        {
            char ch = path[i];
            if (ch == '.')
            {
                if (i != length - 1)
                {
                    return path.Slice(i, length - i);
                }

                return ReadOnlySpan<char>.Empty;
            }

            if (IsDirectorySeparator(ch))
            {
                break;
            }
        }

        return ReadOnlySpan<char>.Empty;
    }

    public static ReadOnlySpan<char> GetFileName(this ReadOnlySpan<char> path)
    {
        for (var i = path.Length; --i >= 0;)
        {
            if (IsDirectorySeparator(path[i]))
            {
                return path.Slice(i + 1, path.Length - i - 1);
            }
        }

        return path;
    }

#else

    public static ReadOnlySpan<char> GetExtension(ReadOnlySpan<char> path) =>
        Path.GetExtension(path);

    public static ReadOnlySpan<char> GetFileName(this ReadOnlySpan<char> path) =>
        Path.GetFileName(path);

#endif
}