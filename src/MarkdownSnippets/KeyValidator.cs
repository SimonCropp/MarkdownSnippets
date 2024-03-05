static class KeyValidator
{
    public static bool IsValidKey(CharSpan key)
    {
        if (key.Length == 0)
        {
            return false;
        }

        if (!char.IsLetterOrDigit(key[0]))
        {
            return false;
        }

        if (!char.IsLetterOrDigit(key[^1]))
        {
            return false;
        }

        return true;
    }
}