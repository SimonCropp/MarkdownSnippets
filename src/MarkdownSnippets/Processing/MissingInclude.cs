namespace MarkdownSnippets;

/// <summary>
/// Part of <see cref="ProcessResult"/>.
/// </summary>
[DebuggerDisplay("Key={Key}, Line={LineNumber}")]
public class MissingInclude
{
    /// <summary>
    /// Initialise a new instance of <see cref="MissingInclude"/>.
    /// </summary>
    public MissingInclude(string key, int lineNumber, string? file)
    {
        Guard.AgainstNullAndEmpty(key, nameof(key));
        Guard.AgainstNegativeAndZero(lineNumber, nameof(lineNumber));
        Guard.AgainstEmpty(file, nameof(file));
        Key = key;
        LineNumber = lineNumber;
        File = file;
    }

    /// <summary>
    /// The key of the missing include.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// The line number in the input text where the include was expected to be injected.
    /// </summary>
    public int LineNumber { get; }

    /// <summary>
    /// The File of the missing include.
    /// </summary>
    public string? File { get; }

    public override string ToString()
    {
        if (File == null)
        {
            return $"""
                    MissingInclude.
                      LineNumber: {LineNumber}
                      Key: {Key}
                    """;
        }

        return $"""
                MissingInclude.
                  File: {File}
                  LineNumber: {LineNumber}
                  Key: {Key}
                """;
    }
}