[DebuggerDisplay("Depth={stack.Count}, IsInSnippet={IsInSnippet}")]
class LoopStack
{
    public bool IsInSnippet => stack.Count > 0;

    public LoopState Current => stack.Peek();

    public void AppendLine(string line)
    {
        foreach (var state in stack)
        {
            state.AppendLine(line);
        }
    }

    public void Pop() => stack.Pop();

    public void Push(EndFunc endFunc, string key, int startLine, int maxWidth, string newLine, string? expressiveCode)
    {
        var state = new LoopState(key, endFunc, startLine, maxWidth, newLine, expressiveCode);
        stack.Push(state);
    }

    Stack<LoopState> stack = [];
}