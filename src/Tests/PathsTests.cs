public class PathsTests
{
    [Theory]
    [InlineData("file.md", true)]
    [InlineData("file.mdx", true)]
    [InlineData("file.MD", true)]
    [InlineData("file.MDX", true)]
    [InlineData("file.Md", true)]
    [InlineData("file.txt", false)]
    [InlineData("file.mdxx", false)]
    public void IsMdFile(string value, bool expected) =>
        Assert.Equal(expected, value.IsMdFile());

    [Theory]
    [InlineData("file.source.md", true)]
    [InlineData("file.source.mdx", true)]
    [InlineData("file.SOURCE.MD", true)]
    [InlineData("file.Source.Mdx", true)]
    [InlineData("file.md", false)]
    [InlineData("file.mdx", false)]
    public void IsSourceMdFile(string value, bool expected) =>
        Assert.Equal(expected, value.IsSourceMdFile());

    [Theory]
    [InlineData("file.include.md", true)]
    [InlineData("file.INCLUDE.MD", true)]
    [InlineData("file.Include.Md", true)]
    [InlineData("file.include.mdx", false)]
    [InlineData("file.md", false)]
    public void IsIncludeMdFile(string value, bool expected) =>
        Assert.Equal(expected, value.IsIncludeMdFile());
}
