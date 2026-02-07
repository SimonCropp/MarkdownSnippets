static class Paths
{
    public static bool IsMdFile(this string value) =>
        value.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
        value.EndsWith(".mdx", StringComparison.OrdinalIgnoreCase);

    public static bool IsSourceMdFile(this string value) =>
        value.EndsWith(".source.md", StringComparison.OrdinalIgnoreCase) ||
        value.EndsWith(".source.mdx", StringComparison.OrdinalIgnoreCase);

    public static bool IsIncludeMdFile(this string value) =>
        value.EndsWith(".include.md", StringComparison.OrdinalIgnoreCase);
}