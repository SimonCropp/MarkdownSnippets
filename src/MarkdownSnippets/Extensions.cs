using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class Extensions
{
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

    public static IReadOnlyList<T> ToReadonlyList<T>(this IEnumerable<T> value)
    {
        return value.ToList();
    }

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
            if (char.IsLetterOrDigit(ch) || ch == ']' || ch == ' ' || ch == ')')
            {
                return input.Substring(startIndex,  index + 1 - startIndex);
            }
        }
        return string.Empty;
    }

    public static string[] SplitBySpace(this string substring)
    {
        return substring
            .Split(new[]
            {
                ' '
            }, StringSplitOptions.RemoveEmptyEntries);
    }

    public static bool IsWhiteSpace(this string target)
    {
        return string.IsNullOrWhiteSpace(target);
    }

    internal static string ReplaceCaseless(this string str, string oldValue, string newValue)
    {
        var stringBuilder = StringBuilderCache.Acquire();
        try
        {
            var previousIndex = 0;
            var index = str.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
            while (index != -1)
            {
                stringBuilder.Append(str.Substring(previousIndex, index - previousIndex));
                stringBuilder.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, StringComparison.OrdinalIgnoreCase);
            }
            stringBuilder.Append(str.Substring(previousIndex));

            return stringBuilder.ToString();
        }
        finally
        {
            StringBuilderCache.Release(stringBuilder);
        }
    }
}