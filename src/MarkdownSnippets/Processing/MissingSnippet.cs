using System.Diagnostics;

namespace MarkdownSnippets
{
    /// <summary>
    /// Part of <see cref="ProcessResult"/>.
    /// </summary>
    [DebuggerDisplay("Key={Key}, Line={Line}")]
    public class MissingSnippet
    {
        /// <summary>
        /// Initialise a new instance of <see cref="MissingSnippet"/>.
        /// </summary>
        public MissingSnippet(string key, int line, string file)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNegativeAndZero(line, nameof(line));
            Guard.AgainstEmpty(file, nameof(file));
            Key = key;
            Line = line;
            File = file;
        }

        /// <summary>
        /// The key of the missing snippet.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The line number in the input text where the snippet was expected to be injected.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// The File of the missing snippet.
        /// </summary>
        public string File { get; }

        public override string ToString()
        {
            if (File == null)
            {
                return $@"MissingSnippet.
  Line: {Line}
  Key: {Key}";
            }

            return $@"MissingSnippet.
  File: {File}
  Line: {Line}
  Key: {Key}";
        }
    }
}