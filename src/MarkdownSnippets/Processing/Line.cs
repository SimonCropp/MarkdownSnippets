using System;

class Line
{
    public Line(string original, string path, int lineNumber)
    {
        Original = original;
        Current = original;
        Path = path;
        LineNumber = lineNumber;
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

    public string Path { get; }
    public int LineNumber { get; }

    public int Length{ get;private set; }

    string current;

    public bool IsWhiteSpace{ get; private set; }
}