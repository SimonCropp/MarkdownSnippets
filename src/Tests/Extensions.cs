using System;
using System.IO;
using System.Reflection;

public static class Extensions
{
    public static string ToCurrentDirectory(this string relativePath)
    {
        return Assembly.GetExecutingAssembly().ToCurrentDirectory(relativePath);
    }

    public static string ToCurrentDirectory(this Assembly assembly, string relativePath)
    {
        var fullPath = new Uri(assembly.CodeBase!).AbsolutePath;
        var directory = Path.GetDirectoryName(fullPath)!;
        return Path.Combine(directory, relativePath);
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