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

    public Line WithCurrent(string current) =>
        new(Original, Path, LineNumber)
        {
            Current = current
        };

    public readonly string Original;

    public override string ToString() =>
        throw new();

    public string Current
    {
        get;
        set
        {
            IsWhiteSpace = value.IsWhiteSpace();
            Length = value.Length;
            field = value;
        }
    } = null!;

    public string? Path { get; }
    public int LineNumber { get; }

    public int Length { get; private set; }

    public bool IsWhiteSpace { get; private set; }
}