using VerifyXunit;
using Xunit;

[UsesVerify]
public class LoopStateTests
{
    [Fact]
    public Task TrimIndentation()
    {
        LoopState loopState = new("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("   Line1");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   Line2");
        return Verifier.Verify(loopState.GetLines());
    }

    [Fact]
    public Task ExcludeEmptyPaddingLines()
    {
        LoopState loopState = new("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("   ");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   ");
        return Verifier.Verify(loopState.GetLines());
    }

    [Fact]
    public Task TrimIndentation_with_mis_match()
    {
        LoopState loopState = new("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("      Line2");
        loopState.AppendLine("   ");
        loopState.AppendLine("     Line4");
        return Verifier.Verify(loopState.GetLines());
    }

    [Fact]
    public void ExcludeEmptyPaddingLines_empty_list()
    {
        LoopState loopState = new("key", _ => throw new(), 1, int.MaxValue, "\n");
        Assert.Empty(loopState.GetLines());
    }

    [Fact]
    public void ExcludeEmptyPaddingLines_whitespace_list()
    {
        LoopState loopState = new("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("");
        loopState.AppendLine("  ");
        Assert.Empty(loopState.GetLines());
    }

    [Fact]
    public Task TrimIndentation_no_initial_padding()
    {
        LoopState loopState = new("key", _ => throw new(), 1, int.MaxValue, "\n");
        loopState.AppendLine("Line1");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   Line2");
        return Verifier.Verify(loopState.GetLines());
    }
}