class KeyValidator
{
    public static bool IsInValidKey(string key) =>
        key.Contains(" ") ||
        !char.IsLetterOrDigit(key, 0) ||
        !char.IsLetterOrDigit(key, key.Length - 1);
}