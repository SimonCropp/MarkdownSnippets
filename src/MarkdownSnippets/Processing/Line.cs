using System;
using System.Diagnostics;

[DebuggerDisplay("Line={LineNumber}, Original={Original}, Current={Current}")]
class Line
{
    public Line(string original, string? path, int lineNumber)
    {
        Original = original;
        Current = original;
        Path = path;
        LineNumber = lineNumber;
    }

    public Line WithCurrent(string current)
    {
        return new (Original, Path, LineNumber)
        {
            Current = current
        };
    }

    public readonly string Original;

    public override string ToString()
    {
        throw new Exception();
    }

    public string Current
    {
        get => current;
        set
        {
            IsWhiteSpace = value.IsWhiteSpace();
            Length = value.Length;
            current = value;
        }
    }

    public string? Path { get; }
    public int LineNumber { get; }

    public int Length { get; private set; }

    string current = null!;

    public bool IsWhiteSpace { get; private set; }
}