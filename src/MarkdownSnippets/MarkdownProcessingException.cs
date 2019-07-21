using System;

namespace MarkdownSnippets
{
    public class MarkdownProcessingException :
        Exception
    {
        public string File { get; }
        public int LineNumber { get; }

        public MarkdownProcessingException(string message, string file, int lineNumber) :
            base(message)
        {
            Guard.AgainstNegativeAndZero(lineNumber, nameof(lineNumber));
            Guard.AgainstEmpty(file, nameof(file));
            File = file;
            LineNumber = lineNumber;
        }

        public override string ToString()
        {
            return $"{Message} File: {File}. LineNumber: {LineNumber}.";
        }
    }
}