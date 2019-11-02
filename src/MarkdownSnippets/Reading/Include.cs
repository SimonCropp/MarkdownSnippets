using System.Collections.Generic;
using System.Diagnostics;

namespace MarkdownSnippets
{
    [DebuggerDisplay("Key={Key}, Path={Path}, Error={Error}")]
    public class Include
    {
        /// <summary>
        /// Initialise a new instance of an in-error <see cref="Include"/>.
        /// </summary>
        public static Include BuildError(string key, int lineNumberInError, string path, string error)
        {
            Guard.AgainstNegativeAndZero(lineNumberInError, nameof(lineNumberInError));
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNullAndEmpty(error, nameof(error));
            return new Include
            {
                Key = key,
                IsInError = true,
                Path = path,
                Error = error
            };
        }

        /// <summary>
        /// Initialise a new instance of <see cref="Include"/>.
        /// </summary>
        public static Include Build(string key, IReadOnlyList<string> lines, string? path)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstEmpty(path, nameof(path));
            Guard.AgainstNull(lines, nameof(lines));
            return new Include
            {
                lines = lines,
                Key = key,
                Path = path,
                Error = ""
            };
        }

        public string Error { get; private set; } = null!;

        public bool IsInError { get; private set; }

        /// <summary>
        /// The key used to identify the snippet.
        /// </summary>
        public string Key { get; private set; } = null!;

        /// <summary>
        /// The path the snippet was read from.
        /// </summary>
        public string? Path { get; private set; }

        public IReadOnlyList<string> Lines
        {
            get
            {
                ThrowIfIsInError();
                return lines!;
            }
        }

        IReadOnlyList<string>? lines;

        void ThrowIfIsInError()
        {
            if (IsInError)
            {
                throw new SnippetReadingException($"Cannot access when {nameof(IsInError)}. Key: {Key}. Path: {Path}. Error: {Error}");
            }
        }

        public override string ToString()
        {
            return $@"ReadInclude.
  Key: {Key}
  Path: {Path}
  Error: {Error}
";
        }
    }
}