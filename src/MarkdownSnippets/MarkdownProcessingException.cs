using System;

namespace CaptureSnippets
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