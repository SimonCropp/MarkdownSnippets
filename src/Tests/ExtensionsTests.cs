[UsesVerify]
public class ExtensionsTests
{
    [Theory]
    [InlineData("a")]
    [InlineData("simple")]
    [InlineData("with Space")]
    [InlineData("with  Multiple  Spaces")]
    [InlineData("  withOuterSpaces  ")]
    [InlineData("  a  ")]
    [InlineData("  ")]
    public Task Simple(string value)
    {
        var spans = value.AsSpan().SplitBySpace();
        var list = new List<string>();
        for (int index = 0; index < spans.Length; index++)
        {
            list.Add(spans[index].ToString());
        }
        return Verify(list)
            .UseParameters(value);
    }
}