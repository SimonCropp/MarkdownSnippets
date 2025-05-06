// ReSharper disable once PartialTypeWithSinglePart
static partial class ExpressiveCode
{
    const string regex = @"([a-zA-Z0-9\-_]+)(?:\((.*?)\))?";

#if NET8_0_OR_GREATER
    public static Regex Pattern { get; } = BuildRegex();

    [GeneratedRegex(regex)]
    private static partial Regex BuildRegex();
#else
    public static readonly Regex Pattern = new(regex);
#endif
}