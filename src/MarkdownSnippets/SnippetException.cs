using System;

namespace MarkdownSnippets
{
    public class SnippetException :
        Exception
    {
        protected SnippetException(string message) :
            base(message)
        {
        }
    }
}