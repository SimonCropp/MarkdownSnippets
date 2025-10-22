static class Extensions
{
    public static bool TryFindNewline(this TextReader reader, [NotNullWhen(true)] out string? newline)
    {
        do
        {
            var c = reader.Read();
            if (c == -1)
            {
                break;
            }

            if (c == '\r')
            {
                var peek = reader.Peek();
                if (peek == -1)
                {
                    newline = "\r";
                    return true;
                }

                if (peek == '\n')
                {
                    newline = "\r\n";
                    return true;
                }

                newline = "\r";
                return true;
            }

            if (c == '\n')
            {
                newline = "\n";
                return true;
            }

        } while (true);

        newline = null;
        return false;
    }

    public static void TrimEnd(this StringBuilder builder)
    {
        var i = builder.Length - 1;
        for (; i >= 0; i--)
        {
            if (!char.IsWhiteSpace(builder[i]))
            {
                break;
            }
        }

        if (i < builder.Length - 1)
        {
            builder.Length = i + 1;
        }
    }

    public static IReadOnlyList<T> ToReadonlyList<T>(this IEnumerable<T> value) => value.ToList();

    public static int LineCount(this CharSpan input)
    {
        var count = 1;
        var len = input.Length;
        for (var i = 0; i != len; ++i)
        {
            switch (input[i])
            {
                case '\r':
                    ++count;
                    if (i + 1 != len && input[i + 1] == '\n')
                    {
                        ++i;
                    }

                    break;
                case '\n':
                    ++count;
                    break;
            }
        }

        return count;
    }

    public static int LastIndexOfSequence(this CharSpan value, char c, int max)
    {
        var index = 0;
        while (true)
        {
            if (index == max)
            {
                return index;
            }

            if (index == value.Length)
            {
                return index;
            }

            var ch = value[index];
            if (c != ch)
            {
                return index;
            }

            index++;
        }
    }

    public static CharSpan TrimBackCommentChars(this CharSpan input, int startIndex)
    {
        for (var index = input.Length - 1; index >= startIndex; index--)
        {
            var ch = input[index];
            if (char.IsLetterOrDigit(ch) || ch is ']' or ' ' or ')')
            {
                return input[startIndex..(index + 1)];
            }
        }

        return string.Empty;
    }

    public static string[] Lines(this string value) =>
        value.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);

    public static bool IsWhiteSpace(this CharSpan target)
    {
        for (var i = 0; i < target.Length; i++)
        {
            if (!char.IsWhiteSpace(target[i]))
            {
                return false;
            }
        }

        return true;
    }
}