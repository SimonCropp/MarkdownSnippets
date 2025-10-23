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

    public void Push(EndFunc endFunc, CharSpan key, int startLine, int maxWidth, string newLine, CharSpan expressiveCode)
    {
        var expressiveCodeString = expressiveCode.Length == 0 ? null : expressiveCode.ToString();

        var state = new LoopState(key.ToString(), endFunc, startLine, maxWidth, newLine, expressiveCodeString);
        stack.Push(state);
    }

    Stack<LoopState> stack = [];
}