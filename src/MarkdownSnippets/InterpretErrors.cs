using System.Collections.Generic;
using System.Linq;

namespace MarkdownSnippets
{
    /// <summary>
    /// Extension method to convert various error cases.
    /// </summary>
    public static class InterpretErrors
    {
        /// <summary>
        /// Converts <see cref="IEnumerable{Snippet}"/> to a markdown string.
        /// </summary>
        public static string ErrorsAsMarkdown(this IReadOnlyList<Snippet> snippets)
        {
            Guard.AgainstNull(snippets, nameof(snippets));
            if (!snippets.Any())
            {
                return "";
            }
            var builder = StringBuilderCache.Acquire();
            builder.AppendLine("## Snippet errors\r\n");
            foreach (var error in snippets)
            {
                builder.AppendLine(" * " + error);
            }
            builder.AppendLine();
            return StringBuilderCache.GetStringAndRelease(builder);
        }

        /// <summary>
        /// Converts <see cref="ProcessResult.MissingSnippets"/> to a markdown string.
        /// </summary>
        public static string ErrorsAsMarkdown(this ProcessResult processResult)
        {
            Guard.AgainstNull(processResult, nameof(processResult));
                var builder = StringBuilderCache.Acquire();
            var missingSnippets = processResult.MissingSnippets.ToList();
            if (missingSnippets.Any())
            {
                builder.AppendLine("## Missing snippets\r\n");
                foreach (var error in missingSnippets)
                {
                    builder.AppendLine($" * Key:'{error.Key}' Line:'{error.LineNumber}'");
                }
            }
            //TODO: handle other errors

            builder.AppendLine();
            return StringBuilderCache.GetStringAndRelease(builder);
        }
    }
}