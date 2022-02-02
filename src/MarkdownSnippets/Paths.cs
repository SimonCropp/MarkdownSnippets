static class Paths
{
    public static bool IsMdFile(this string value)
    {
        return value.EndsWith(".md") ||
               value.EndsWith(".mdx") ;
    }

    public static bool IsSourceMdFile(this string value)
    {
        return value.EndsWith(".source.md") ||
               value.EndsWith(".source.mdx");
    }

    public static bool IsIncludeMdFile(this string value)
    {
        return value.EndsWith(".include.md");
    }
}