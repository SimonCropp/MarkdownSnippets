using System;

namespace CaptureSnippets
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