namespace MarkdownSnippets;

public class ContentValidationException(IReadOnlyList<ValidationError> errors) :
    SnippetException(BuildMessage(errors))
{
    public IReadOnlyList<ValidationError> Errors { get; } = errors;

    static string BuildMessage(IReadOnlyList<ValidationError> errors)
    {
        var builder = new StringBuilder("Content validation errors:");
        builder.AppendLine();
        foreach (var error in errors)
        {
            if (error.File == null)
            {
                Polyfill.AppendLine(
                    builder,
                    $"""
                     {error.Error}
                       Line: {error.Line}
                       Column: {error.Column}
                     """);
            }

            Polyfill.AppendLine(
                builder,
                $"""
                 {error.Error}
                   File: {error.File}
                   Line: {error.Line}
                   Column: {error.Column}
                 """);
        }

        return builder.ToString();
    }

    public override string ToString() => Message;
}