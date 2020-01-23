using System;

namespace MarkdownSnippets
{
    public class SnippetException :
        Exception
    {
        public SnippetException(string message) :
            base(message)
        {
        }
    }
}