static class Paths
{
    public static bool IsMdFile(this string value)
    {
        return value.EndsWith(".md");
    }

    public static bool IsSourceMdFile(this string value)
    {
        return value.EndsWith(".source.md");
    }

    public static bool IsIncludeMdFile(this string value)
    {
        return value.EndsWith(".include.md");
    }
}