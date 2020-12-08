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

    public void Push(Func<string, bool> endFunc, string key, int startLine, int maxWidth, string newLine)
    {
        LoopState state = new(key, endFunc, startLine, maxWidth, newLine);
        stack.Push(state);
    }

    Stack<LoopState> stack = new();
}