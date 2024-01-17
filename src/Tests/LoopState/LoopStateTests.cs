public class LoopStateTests
{
    [Fact]
    public Task TrimIndentation()
    {
        var loopState = new LoopState("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("   Line1");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   Line2");
        return Verify(loopState.GetLines());
    }

    [Fact]
    public Task ExcludeEmptyPaddingLines()
    {
        var loopState = new LoopState("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("   ");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   ");
        return Verify(loopState.GetLines());
    }

    [Fact]
    public Task TrimIndentation_with_mis_match()
    {
        var loopState = new LoopState("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("      Line2");
        loopState.AppendLine("   ");
        loopState.AppendLine("     Line4");
        return Verify(loopState.GetLines());
    }

    [Fact]
    public void ExcludeEmptyPaddingLines_empty_list()
    {
        var loopState = new LoopState("key", _ => throw new(), 1, int.MaxValue, "\n");
        Assert.Empty(loopState.GetLines());
    }

    [Fact]
    public void ExcludeEmptyPaddingLines_whitespace_list()
    {
        var loopState = new LoopState("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("");
        loopState.AppendLine("  ");
        Assert.Empty(loopState.GetLines());
    }

    [Fact]
    public Task TrimIndentation_no_initial_padding()
    {
        var loopState = new LoopState("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("Line1");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   Line2");
        return Verify(loopState.GetLines());
    }
}