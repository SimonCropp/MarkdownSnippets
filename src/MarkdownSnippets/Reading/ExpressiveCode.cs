static partial class ExpressiveCode
{
#if NET8_0_OR_GREATER
    public static Regex Pattern { get; } = BuildRegex();

    [GeneratedRegex(@"([a-zA-Z0-9\-_]+)(?:\((.*?)\))?")]
    private static partial Regex BuildRegex();
#else
    public static readonly Regex Pattern = new(@"([a-zA-Z0-9\-_]+)(?:\((.*?)\))?");
#endif
}