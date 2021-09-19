namespace MarkdownSnippets
{
    [DebuggerDisplay("Key={Key}, FileLocation={FileLocation}, Error={Error}")]
    public class Snippet : IContent
    {
        /// <summary>
        /// Initialise a new instance of an in-error <see cref="Snippet"/>.
        /// </summary>
        public static Snippet BuildError(string key, int lineNumberInError, string path, string error)
        {
            Guard.AgainstNegativeAndZero(lineNumberInError, nameof(lineNumberInError));
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstNullAndEmpty(error, nameof(error));
            return new()
            {
                Key = key,
                StartLine = lineNumberInError,
                EndLine = lineNumberInError,
                IsInError = true,
                Path = path,
                Error = error
            };
        }

        /// <summary>
        /// Initialise a new instance of <see cref="Snippet"/>.
        /// </summary>
        public static Snippet Build(int startLine, int endLine, string value, string key, string language, string? path)
        {
            Guard.AgainstNullAndEmpty(key, nameof(key));
            Guard.AgainstEmpty(path, nameof(path));
            Guard.AgainstUpperCase(language, nameof(language));
            if (language.StartsWith("."))
            {
                throw new ArgumentException("Language cannot start with '.'", nameof(language));
            }

            Guard.AgainstNegativeAndZero(startLine, nameof(startLine));
            Guard.AgainstNegativeAndZero(endLine, nameof(endLine));
            return new()
            {
                StartLine = startLine,
                EndLine = endLine,
                value = value,
                Key = key,
                language = language,
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
        /// The language of the snippet, extracted from the file extension of the input file.
        /// </summary>
        public string Language
        {
            get
            {
                ThrowIfIsInError();
                return language!;
            }
        }
        string? language;

        /// <summary>
        /// The path the snippet was read from.
        /// </summary>
        public string? Path { get; private set; }

        /// <summary>
        /// The line the snippets started on.
        /// </summary>
        public int StartLine { get; private set; }

        /// <summary>
        /// The line the snippet ended on.
        /// </summary>
        public int EndLine { get; private set; }

        /// <summary>
        /// The <see cref="Path"/>, <see cref="StartLine"/>, and <see cref="EndLine"/> concatenated.
        /// </summary>
        public string? FileLocation
        {
            get
            {
                if (Path == null)
                {
                    return null;
                }
                return $"{Path}({StartLine}-{EndLine})";
            }
        }

        public string Value
        {
            get
            {
                ThrowIfIsInError();
                return value!;
            }
        }

        string? value;

        void ThrowIfIsInError()
        {
            if (IsInError)
            {
                throw new SnippetReadingException($"Cannot access when {nameof(IsInError)}. Key: {Key}. FileLocation: {FileLocation}. Error: {Error}");
            }
        }

        public override string ToString()
        {
            return $@"ReadSnippet.
  Key: {Key}
  FileLocation: {FileLocation}
  Error: {Error}
";
        }
    }

    public interface IContent
    {
        string? Path { get; }
    }
}