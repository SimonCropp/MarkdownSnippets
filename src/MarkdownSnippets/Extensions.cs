#if NETSTANDARD
using System.Runtime.InteropServices;
#endif

static class Extensions
{
    public static bool StartsWith(this CharSpan value, char ch) =>
        value.Length != 0 &&
        value[0] == ch;

    public static bool Contains(this CharSpan target, CharSpan value) =>
        target.IndexOf(value) != -1;

    public static CharSpan Replace(this CharSpan target, char from, char to)
    {
        var chars = new char[target.Length];
        for (var index = 0; index < target.Length; index++)
        {
            var ch = target[index];
            if (ch == from)
            {
                chars[index] = to;
            }
            else
            {
                chars[index] = ch;
            }
        }

        return new(chars);
    }

    public static bool TryFindNewline(this TextReader reader, out string? newline)
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

    public static int LineCount(this string input)
    {
        var count = 1;
        var len = input.Length;
        for(var i = 0; i != len; ++i)
        {
            switch(input[i])
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

    public static int LastIndexOfSequence(this string value, char c, int max)
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
                return input.Slice(startIndex,  index + 1 - startIndex);
            }
        }

        return CharSpan.Empty;
    }

    public static bool EndsWith(this CharSpan value, char ch)
    {
        var lastPos = value.Length - 1;
        if (lastPos == -1)
        {
            return false;
        }

        return value[lastPos] == ch;
    }

#if NETSTANDARD

    public static bool SequenceEqual(this CharSpan value1, CharSpan value2)
    {
        if (value1.Length != value2.Length)
        {
            return false;
        }

        for (var index = 0; index < value1.Length; index++)
        {
            var ch1 = value1[index];
            var ch2 = value2[index];
            if (ch1 != ch2)
            {
                return false;
            }
        }

        return true;
    }

    public static bool StartsWith(this string value, char ch) =>
        value.Length != 0 && value[0] == ch;

    public static bool EndsWith(this string value, char ch)
    {
        if (value.Length == 0)
        {
            return false;
        }

        var lastPos = value.Length - 1;
        return lastPos < value.Length &&
               value[lastPos] == ch;
    }

    public static bool Contains(this CharSpan span, char value)
    {
        foreach (var ch in span)
        {
            if (ch == value)
            {
                return true;
            }
        }

        return false;
    }

    public static void Append(this StringBuilder builder, CharSpan value)
    {
        if (value.Length > 0)
        {
            unsafe
            {
                fixed (char* valueChars = &MemoryMarshal.GetReference(value))
                {
                    builder.Append(valueChars, value.Length);
                }
            }
        }
    }
#endif

    public static string[] Lines(this string value) =>
        value.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

    public static bool IsWhiteSpace(this string target) =>
        string.IsNullOrWhiteSpace(target);
}