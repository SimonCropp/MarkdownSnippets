using System;
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

        Approvals.Verify(TocBuilder.BuildToc(lines, 1, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public void IgnoreTop()
    {
        var lines = new List<Line>
        {
            new Line("# Heading1", "", 0),
            new Line("## Heading2", "", 0)
        };

        Approvals.Verify(TocBuilder.BuildToc(lines, 1, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public void SanitizeLink()
    {
        Approvals.Verify(TocBuilder.SanitizeLink("A!@#$%,^&*()_+-={};':\"<>?/b"));
    }

    [Fact]
    public void StripMarkdown()
    {
        var lines = new List<Line>
        {
            new Line("## **bold** *italic* [Link](link)", "", 0)
        };

        Approvals.Verify(TocBuilder.BuildToc(lines, 1, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public void Exclude()
    {
        var lines = new List<Line>
        {
            new Line("## Heading1", "", 0),
            new Line("### Heading2", "", 0)
        };

        Approvals.Verify(TocBuilder.BuildToc(lines, 1, new List<string> {"Heading2"}, Environment.NewLine));
    }

    [Fact]
    public void Nested()
    {
        var lines = new List<Line>
        {
            new Line("## Heading1", "", 0),
            new Line("### Heading2", "", 0),
            new Line("## Heading3", "", 0),
            new Line("### Heading4", "", 0)
        };

        Approvals.Verify(TocBuilder.BuildToc(lines, 2, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public void Deep()
    {
        var lines = new List<Line>
        {
            new Line("## Heading1", "", 0),
            new Line("### Heading2", "", 0),
            new Line("#### Heading3", "", 0),
            new Line("##### Heading4", "", 0)
        };

        Approvals.Verify(TocBuilder.BuildToc(lines, 10, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public void StopAtLevel()
    {
        var lines = new List<Line>
        {
            new Line("## Heading1", "", 0),
            new Line("### Heading2", "", 0),
            new Line("#### Heading3", "", 0)
        };

        Approvals.Verify(TocBuilder.BuildToc(lines, 2, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public void Single()
    {
        var lines = new List<Line>
        {
            new Line("## Heading", "", 0)
        };

        Approvals.Verify(TocBuilder.BuildToc(lines, 1, new List<string>(), Environment.NewLine));
    }

    [Fact]
    public void WithSpaces()
    {
        var lines = new List<Line>
        {
            new Line("##  A B ", "", 0)
        };

        Approvals.Verify(TocBuilder.BuildToc(lines, 1, new List<string>(), Environment.NewLine));
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

        Approvals.Verify(TocBuilder.BuildToc(lines, 1, new List<string>(), Environment.NewLine));
    }

    public TocBuilderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}