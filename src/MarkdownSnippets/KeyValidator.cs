class KeyValidator
{
    public static bool IsInValidKey(string key)
    {
        return key.Contains(" ") ||
               !char.IsLetterOrDigit(key, 0) ||
               !char.IsLetterOrDigit(key, key.Length - 1);
    }
}