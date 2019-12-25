using System;

class LineTooLongException :
    Exception
{
    public string Line { get; }

    public LineTooLongException(string line)
    {
        Line = line;
    }
}