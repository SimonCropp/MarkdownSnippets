using System;

namespace MarkdownSnippets
{
    public class MarkdownProcessingException :
        Exception
    {
        public MarkdownProcessingException(string message) :
            base(message)
        {
        }
    }
}