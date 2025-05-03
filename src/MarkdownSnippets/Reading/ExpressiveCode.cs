static partial class ExpressiveCode
{
#if NET8_0_OR_GREATER
    static ExpressiveCode() =>
        Pattern = KeyPatternRegex();

    public static Regex Pattern { get; }

    [GeneratedRegex(@"([a-zA-Z0-9\-_]+)(?:\((.*?)\))?")]
    private static partial Regex KeyPatternRegex();
#else
    public static readonly Regex Pattern = new(@"([a-zA-Z0-9\-_]+)(?:\((.*?)\))?");
#endif
}