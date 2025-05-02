internal partial class ExpressiveCode
{
#if NET8_0_OR_GREATER
    [GeneratedRegex(@"([a-zA-Z0-9\-_]+)(?:\((.*?)\))?")]
    protected partial Regex KeyPatternRegex();
#else
    protected static readonly Regex KeyPatternRegex = new(@"([a-zA-Z0-9\-_]+)(?:\((.*?)\))?");
#endif
    public ExpressiveCode() =>
#if NET8_0_OR_GREATER
        Pattern = KeyPatternRegex();
#else
        Pattern = KeyPatternRegex;
#endif
    public static ExpressiveCode Instance { get; } = new();
    public Regex Pattern { get; }
}