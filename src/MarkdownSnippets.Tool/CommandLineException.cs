using System;

class CommandLineException :
    Exception
{
    public CommandLineException(string message) :
        base(message)
    {
    }
}