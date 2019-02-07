using System;
using System.IO;
using System.Linq;

static class Guard
{
    // ReSharper disable UnusedParameter.Global
    public static void AgainstNull(object value, string argumentName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstUpperCase(string value, string argumentName)
    {
        if (value.Any(char.IsUpper))
        {
            throw new ArgumentException("Cannot contain upper case", argumentName);
        }
    }

    public static void AgainstNegativeAndZero(long value, string argumentName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(argumentName);
        }
    }

    public static void AgainstNegativeAndZero(int value, string argumentName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(argumentName);
        }
    }

    public static void AgainstNullAndEmpty(string value, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void DirectoryExists(string path, string argumentName)
    {
        AgainstNullAndEmpty(path,nameof(argumentName));
        if (!Directory.Exists(path))
        {
            throw new ArgumentException($"Directory does not exist: {path}", nameof(argumentName));
        }
    }

    public static void FileExists(string path, string argumentName)
    {
        AgainstNullAndEmpty(path,nameof(argumentName));
        if (!File.Exists(path))
        {
            throw new ArgumentException($"File does not exist: {path}", nameof(argumentName));
        }
    }

    public static void AgainstEmpty(string value, string argumentName)
    {
        if (value == null)
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Cannot be only whitespace.", argumentName);
        }
    }
}