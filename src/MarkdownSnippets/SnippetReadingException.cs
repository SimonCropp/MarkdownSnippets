namespace MarkdownSnippets;

public class SnippetReadingException :
    SnippetException
{
    public SnippetReadingException(string message) :
        base(message)
    {
    }
}