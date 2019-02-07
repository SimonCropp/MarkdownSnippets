using System.Diagnostics;

namespace CaptureSnippets
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
        public MissingSnippet(string key, int line)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNegativeAndZero(line, nameof(line));
            Key = key;
            Line = line;
        }

        /// <summary>
        /// The key of the missing snippet.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The line number in the input text where the snippet was expected to be injected.
        /// </summary>
        public int Line { get; }

        public override string ToString()
        {
            return $@"MissingSnippet.
  Line: {Line}
  Key: {Key}
";
        }
    }
}