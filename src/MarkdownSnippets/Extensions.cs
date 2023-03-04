#if NETSTANDARD
using System.Runtime.InteropServices;
#endif

static class Extensions
{
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

    public static string TrimBackCommentChars(this string input, int startIndex)
    {
        for (var index = input.Length - 1; index >= startIndex; index--)
        {
            var ch = input[index];
            if (char.IsLetterOrDigit(ch) || ch is ']' or ' ' or ')')
            {
                return input.Substring(startIndex,  index + 1 - startIndex);
            }
        }
        return string.Empty;
    }


    public static string[] SplitBySpace(this string value) =>
        value.Split(new[]
                {
                    ' '
                },
                StringSplitOptions.RemoveEmptyEntries);

    public static Spans SplitBySpace(this CharSpan value)
    {
        var indexes = new List<Spans.Index>();
        var index = 0;
        int? from = null;
        while (true)
        {
            if (index == value.Length)
            {
                if (from != null)
                {
                    indexes.Add(new (from.Value, index-from.Value));
                }
                break;
            }
            var ch = value[index];

            if (ch == ' ')
            {
                if (from != null)
                {
                    indexes.Add(new (from.Value, index-from.Value));
                }

                from = null;
            }
            else
            {
                if (from == null)
                {
                    from = index;
                }
            }

            index++;
        }

        return new(value,indexes);
    }

    public static bool StartsWith(this CharSpan value, char ch) =>
        value.Length != 0 && value[0] == ch;

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

    public static bool StartsWith(this string value, char ch) =>
        value.Length != 0 && value[0] == ch;



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

#endif

    public static string[] Lines(this string value) =>
        value.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

    public static bool IsWhiteSpace(this string target) =>
        string.IsNullOrWhiteSpace(target);
}

ref struct Spans
{
    CharSpan span;
    readonly List<Index> items;

    public Spans(CharSpan span, List<Index> items)
    {
        this.span = span;
        this.items = items;
    }

    public record Index(int From, int To);

    public int Length => items.Count;

    public CharSpan this[int index]
    {
        get
        {
            var tuple = items[index];
            return span.Slice(tuple.From, tuple.To);
        }
    }
}