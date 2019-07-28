using System.Collections.Generic;
using ApprovalTests;
using Xunit;
using Xunit.Abstractions;

public class TocBuilderTests :
    TestBase
{
    [Fact]
    public void EmptyHeading()
    {
        var lines = new List<Line>
        {
            new Line("##", "", 0)
        };

        Approvals.Verify(TocBuilder.BuildToc(lines,1));
    }

    [Fact]
    public void IgnoreTop()
    {
        var lines = new List<Line>
        {
            new Line("# Heading1", "", 0),
            new Line("## Heading2", "", 0)
        };

        Approvals.Verify(TocBuilder.BuildToc(lines,1));
    }

    [Fact]
    public void Single()
    {
        var lines = new List<Line>
        {
            new Line("## Heading", "", 0)
        };

        Approvals.Verify(TocBuilder.BuildToc(lines,1));
    }

    [Fact]
    public void WithSpaces()
    {
        var lines = new List<Line>
        {
            new Line("##  A B ", "", 0)
        };

        Approvals.Verify(TocBuilder.BuildToc(lines,1));
    }

    [Fact]
    public void Duplicates()
    {
        var lines = new List<Line>
        {
            new Line("## A", "", 0),
            new Line("## A", "", 0),
            new Line("## a", "", 0)
        };

        Approvals.Verify(TocBuilder.BuildToc(lines,1));
    }

    public TocBuilderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}