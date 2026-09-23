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

    static string[] agentFilePrefixes =
    [
        "CLAUDE.",
        "AGENTS.",
        "GEMINI."
    ];

    static string[] agentFileSuffixes =
    [
        ".instructions.md",
        ".prompt.md",
        ".chatmode.md",
        ".agent.md"
    ];

    public static bool IsAgentFile(this string value)
    {
        var name = Path.GetFileName(value);
        if (name.Equals("copilot-instructions.md", StringComparison.OrdinalIgnoreCase) ||
            name.Equals(".cursorrules", StringComparison.OrdinalIgnoreCase) ||
            name.Equals(".windsurfrules", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (agentFileSuffixes.Any(_ => name.EndsWith(_, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return name.EndsWith(".md", StringComparison.OrdinalIgnoreCase) &&
               agentFilePrefixes.Any(_ => name.StartsWith(_, StringComparison.OrdinalIgnoreCase));
    }
}