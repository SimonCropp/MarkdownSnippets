[UsesVerify]
public class TocBuilderTests
{
    [Fact]
    public Task EmptyHeading()
    {
        List<Line> lines = new()
        {
            new("##", "", 0)
        };

        var buildToc = TocBuilder.BuildToc(lines, 1, new(), "\r");
        Assert.DoesNotContain("\r\n", buildToc);
        return Verify(buildToc);
    }

    [Fact]
    public Task IgnoreTop()
    {
        List<Line> lines = new()
        {
            new("# Heading1", "", 0),
            new("## Heading2", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 1, new(), Environment.NewLine));
    }

    [Fact]
    public Task SanitizeLink()
    {
        return Verify(TocBuilder.SanitizeLink("A!@#$%,^&*()_+-={};':\"<>?/b"));
    }

    [Fact]
    public Task StripMarkdown()
    {
        List<Line> lines = new()
        {
            new("## **bold** *italic* [Link](link)", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 1, new(), Environment.NewLine));
    }

    [Fact]
    public Task Exclude()
    {
        List<Line> lines = new()
        {
            new("## Heading1", "", 0),
            new("### Heading2", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 1, new() {"Heading2"}, Environment.NewLine));
    }

    [Fact]
    public Task Nested()
    {
        List<Line> lines = new()
        {
            new("## Heading1", "", 0),
            new("### Heading2", "", 0),
            new("## Heading3", "", 0),
            new("### Heading4", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 2, new(), Environment.NewLine));
    }

    [Fact]
    public Task Deep()
    {
        List<Line> lines = new()
        {
            new("## Heading1", "", 0),
            new("### Heading2", "", 0),
            new("#### Heading3", "", 0),
            new("##### Heading4", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 10, new(), Environment.NewLine));
    }

    [Fact]
    public Task StopAtLevel()
    {
        List<Line> lines = new()
        {
            new("## Heading1", "", 0),
            new("### Heading2", "", 0),
            new("#### Heading3", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 2, new(), Environment.NewLine));
    }

    [Fact]
    public Task Single()
    {
        List<Line> lines = new()
        {
            new("## Heading", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 1, new(), Environment.NewLine));
    }

    [Fact]
    public Task WithSpaces()
    {
        List<Line> lines = new()
        {
            new("##  A B ", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 1, new(), Environment.NewLine));
    }

    [Fact]
    public Task DuplicateNested()
    {
        List<Line> lines = new()
        {
            new("## Heading", "", 0),
            new("### Heading", "", 0),
            new("#### Heading", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines,4, new(), Environment.NewLine));
    }

    [Fact]
    public Task Duplicates()
    {
        List<Line> lines = new()
        {
            new("## A", "", 0),
            new("## A", "", 0),
            new("## a", "", 0)
        };

        return Verify(TocBuilder.BuildToc(lines, 1, new(), Environment.NewLine));
    }
}