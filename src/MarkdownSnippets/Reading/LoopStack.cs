using System;
using System.Collections.Generic;
using System.Diagnostics;

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

    public void Pop()
    {
        stack.Pop();
    }

    public void Push(Func<string, bool> endFunc, string key, int startLine)
    {
        var state = new LoopState
        {
            Key = key,
            EndFunc = endFunc,
            StartLine = startLine,
        };
        stack.Push(state);
    }

    Stack<LoopState> stack = new Stack<LoopState>();
}