[UsesVerify]
public class TocBuilderTests
{
    [Fact]
    public Task EmptyHeading()
    {
        var lines = new List<Line>
        {
            new("##", "", 0)
        };

        var buildToc = TocBuilder.BuildToc(lines, 1, new List<string>(), "\r");
        Assert.DoesNotContain("\r\n", buildToc);
        return Verify(buildToc);
    }

    [Fact]
    public Task IgnoreTop()
    {
        var lines = new List<Line>
        {
            new("# Heading1", "", 0),
            new("## Heading2", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 1, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public Task SanitizeLink()
    {
        return Verify(TocBuilder.SanitizeLink("A!@#$%,^&*()_+-={};':\"<>?/b"));
    }

    [Fact]
    public Task StripMarkdown()
    {
        var lines = new List<Line>
        {
            new("## **bold** *italic* [Link](link)", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 1, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public Task Exclude()
    {
        var lines = new List<Line>
        {
            new("## Heading1", "", 0),
            new("### Heading2", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 1, new List<string> {"Heading2"}, Environment.NewLine));
    }

    [Fact]
    public Task Nested()
    {
        var lines = new List<Line>
        {
            new("## Heading1", "", 0),
            new("### Heading2", "", 0),
            new("## Heading3", "", 0),
            new("### Heading4", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 2, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public Task Deep()
    {
        var lines = new List<Line>
        {
            new("## Heading1", "", 0),
            new("### Heading2", "", 0),
            new("#### Heading3", "", 0),
            new("##### Heading4", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 10, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public Task StopAtLevel()
    {
        var lines = new List<Line>
        {
            new("## Heading1", "", 0),
            new("### Heading2", "", 0),
            new("#### Heading3", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 2, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public Task Single()
    {
        var lines = new List<Line>
        {
            new("## Heading", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 1, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public Task WithSpaces()
    {
        var lines = new List<Line>
        {
            new("##  A B ", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 1, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public Task DuplicateNested()
    {
        var lines = new List<Line>
        {
            new("## Heading", "", 0),
            new("### Heading", "", 0),
            new("#### Heading", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines,4, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public Task Duplicates()
    {
        var lines = new List<Line>
        {
            new("## A", "", 0),
            new("## A", "", 0),
            new("## a", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 1, new List<string>(), Environment.NewLine));
    }
}