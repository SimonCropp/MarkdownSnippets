public class LoopStateTests
{
    [Test]
    public Task TrimIndentation()
    {
        var loopState = new LoopState("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("   Line1");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   Line2");
        return Verify(loopState.GetLines())
            .Snapshot(
                """
                Line1
                 Line2
                Line2
                """);
    }

    [Test]
    public Task ExcludeEmptyPaddingLines()
    {
        var loopState = new LoopState("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("   ");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   ");
        return Verify(loopState.GetLines())
            .Snapshot("Line2");
    }

    [Test]
    public Task TrimIndentation_with_mis_match()
    {
        var loopState = new LoopState("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("      Line2");
        loopState.AppendLine("   ");
        loopState.AppendLine("     Line4");
        return Verify(loopState.GetLines())
            .Snapshot(
                """
                Line2

                Line4
                """);
    }

    [Test]
    public async Task ExcludeEmptyPaddingLines_empty_list()
    {
        var loopState = new LoopState("key", _ => throw new(), 1, int.MaxValue, "\n");
        await Assert.That(loopState.GetLines()).IsEmpty();
    }

    [Test]
    public async Task ExcludeEmptyPaddingLines_whitespace_list()
    {
        var loopState = new LoopState("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("");
        loopState.AppendLine("  ");
        await Assert.That(loopState.GetLines()).IsEmpty();
    }

    [Test]
    public Task TrimIndentation_no_initial_padding()
    {
        var loopState = new LoopState("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("Line1");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   Line2");
        return Verify(loopState.GetLines())
            .Snapshot(
                """
                Line1
                    Line2
                   Line2
                """);
    }
}