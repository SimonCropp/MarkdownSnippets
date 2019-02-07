using System;

namespace MarkdownSnippets
{
    public class SnippetReadingException :
        Exception
    {
        public SnippetReadingException(string message) :
            base(message)
        {
        }
    }
}