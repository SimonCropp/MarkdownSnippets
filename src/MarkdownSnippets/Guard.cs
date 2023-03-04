static class Guard
{
    public static void AgainstUpperCase(string value, string argumentName)
    {
        if (value.Any(char.IsUpper))
        {
            throw new ArgumentException($"Cannot contain upper case. Value: {value}", argumentName);
        }
    }

    public static void AgainstUpperCase(CharSpan value, string argumentName)
    {
        foreach (var ch in value)
        {
            if (char.IsUpper(ch))
            {
                throw new ArgumentException($"Cannot contain upper case. Value: {value.ToString()}", argumentName);
            }
        }
    }

    public static void AgainstNegativeAndZero(int value, string argumentName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(argumentName,value, "Zero or less");
        }
    }

    public static void AgainstNegative(int value, string argumentName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(argumentName, value, "negative");
        }
    }

    public static void AgainstNullAndEmpty(string? value, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstNullAndEmpty(CharSpan value, string argumentName)
    {
        if (value.Contains(' '))
        {
            throw new ArgumentException("Empty span", argumentName);
        }
    }

    public static void DirectoryExists(string path, string argumentName)
    {
        AgainstNullAndEmpty(path, argumentName);
        if (!Directory.Exists(path))
        {
            throw new ArgumentException($"Directory does not exist: {path}", argumentName);
        }
    }

    public static void FileExists(string? path, string argumentName)
    {
        AgainstNullAndEmpty(path, argumentName);
        if (!File.Exists(path))
        {
            throw new ArgumentException($"File does not exist: {path}", argumentName);
        }
    }

    public static void AgainstEmpty(string? value, string argumentName)
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