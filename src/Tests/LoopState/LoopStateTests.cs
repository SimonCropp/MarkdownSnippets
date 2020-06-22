using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class LoopStateTests
{
    [Fact]
    public Task TrimIndentation()
    {
        var loopState = new LoopState("key", s => throw new Exception(), 1, int.MaxValue);
        loopState.AppendLine("   Line1");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   Line2");
        return Verifier.Verify(loopState.GetLines());
    }

    [Fact]
    public Task ExcludeEmptyPaddingLines()
    {
        var loopState = new LoopState("key", s => throw new Exception(), 1, int.MaxValue);
        loopState.AppendLine("   ");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   ");
        return Verifier.Verify(loopState.GetLines());
    }

    [Fact]
    public Task TrimIndentation_with_mis_match()
    {
        var loopState = new LoopState("key", s => throw new Exception(), 1, int.MaxValue);
        loopState.AppendLine("      Line2");
        loopState.AppendLine("   ");
        loopState.AppendLine("     Line4");
        return Verifier.Verify(loopState.GetLines());
    }

    [Fact]
    public void ExcludeEmptyPaddingLines_empty_list()
    {
        var loopState = new LoopState("key", s => throw new Exception(), 1, int.MaxValue);
        Assert.Empty(loopState.GetLines());
    }

    [Fact]
    public void ExcludeEmptyPaddingLines_whitespace_list()
    {
        var loopState = new LoopState("key", s => throw new Exception(), 1, int.MaxValue);
        loopState.AppendLine("");
        loopState.AppendLine("  ");
        Assert.Empty(loopState.GetLines());
    }

    [Fact]
    public Task TrimIndentation_no_initial_padding()
    {
        var loopState = new LoopState("key", s => throw new Exception(), 1, int.MaxValue);
        loopState.AppendLine("Line1");
        loopState.AppendLine("    Line2");
        loopState.AppendLine("   Line2");
        return Verifier.Verify(loopState.GetLines());
    }
}