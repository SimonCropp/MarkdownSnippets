static partial class ExpressiveCode
{
#if NET8_0_OR_GREATER
    [GeneratedRegex(@"([a-zA-Z0-9\-_]+)(?:\((.*?)\))?")]
    private static partial Regex KeyPatternRegex();
#else
    static readonly Regex KeyPatternRegex = new(@"([a-zA-Z0-9\-_]+)(?:\((.*?)\))?");
#endif
    static ExpressiveCode() =>
#if NET8_0_OR_GREATER
        Pattern = KeyPatternRegex();
#else
        Pattern = KeyPatternRegex;
#endif
    public static Regex Pattern { get; }
}