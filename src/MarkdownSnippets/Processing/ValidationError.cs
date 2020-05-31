using System.Diagnostics;

namespace MarkdownSnippets
{
    /// <summary>
    /// Part of <see cref="ProcessResult"/>.
    /// </summary>
    [DebuggerDisplay("Error={Error}, Line={LineNumber:Column}")]
    public class ValidationError
    {
        /// <summary>
        /// Initialise a new instance of <see cref="ValidationError"/>.
        /// </summary>
        public ValidationError(string error, int lineNumber, int column, string? file)
        {
            Guard.AgainstNullAndEmpty(error, nameof(error));
            Guard.AgainstNegativeAndZero(lineNumber, nameof(lineNumber));
            Guard.AgainstNegativeAndZero(column, nameof(column));
            Guard.AgainstEmpty(file, nameof(file));
            Error = Error;
            LineNumber = lineNumber;
            Column = column;
            File = file;
        }

        /// <summary>
        /// The error.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// The line number in the input text.
        /// </summary>
        public int LineNumber { get; }
        /// <summary>
        /// The column number in the line.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// The File.
        /// </summary>
        public string? File { get; }

        public override string ToString()
        {
            if (File == null)
            {
                return $@"ContentError.
  LineNumber: {LineNumber}
  Column: {Column}
  Error: {Error}";
            }

            return $@"ContentError.
  File: {File}
  LineNumber: {LineNumber}
  Column: {Column}
  Error: {Error}";
        }
    }
}