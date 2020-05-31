using System.Collections.Generic;
using System.Text;

namespace MarkdownSnippets
{
    public class ContentValidationException :
        SnippetException
    {
        public IReadOnlyList<ValidationError> Errors { get; }

        public ContentValidationException(IReadOnlyList<ValidationError> errors) :
            base(BuildMessage(errors))
        {
            Errors = errors;
        }

        static string BuildMessage(IReadOnlyList<ValidationError> errors)
        {
            var builder = new StringBuilder("Content validation errors:");
            builder.AppendLine();
            foreach (var error in errors)
            {
                if (error.File == null)
                {
                    builder.AppendLine($@"{error.Error}
  LineNumber: {error.LineNumber}
  Column: {error.Column}
  Error: {error.Error}");
                }

                builder.AppendLine($@"{error.Error}
  File: {error.File}
  LineNumber: {error.LineNumber}
  Column: {error.Column}
  Error: {error.Error}");
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            return Message;
        }
    }
}