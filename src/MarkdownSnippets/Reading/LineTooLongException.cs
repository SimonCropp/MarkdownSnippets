class LineTooLongException(string line) :
    Exception
{
    public string Line { get; } = line;
}