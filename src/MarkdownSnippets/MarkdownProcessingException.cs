namespace MarkdownSnippets;

public class MarkdownProcessingException :
    SnippetException
{
    public string? File { get; }
    public int LineNumber { get; }

    public MarkdownProcessingException(string message, string? file, int lineNumber) :
        base($"{message} File: {file}. LineNumber: {lineNumber}.")
    {
        Guard.AgainstNegativeAndZero(lineNumber, nameof(lineNumber));
        Guard.AgainstEmpty(file, nameof(file));
        File = file;
        LineNumber = lineNumber;
    }

    public override string ToString()
    {
        return Message;
    }
}