using System;

namespace MarkdownSnippets
{
    public class MarkdownProcessingException :
        Exception
    {
        public string File { get; }
        public int Line { get; }

        public MarkdownProcessingException(string message, string file, int line) :
            base(message)
        {
            Guard.AgainstNegativeAndZero(line, nameof(line));
            Guard.AgainstEmpty(file, nameof(file));
            File = file;
            Line = line;
        }
    }
}