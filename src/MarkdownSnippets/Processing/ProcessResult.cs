using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownSnippets
{
    /// <summary>
    /// The result of <see cref="MarkdownProcessor"/> Apply methods.
    /// </summary>
    public class ProcessResult :
        IEnumerable<Snippet>
    {
        public ProcessResult(
            IReadOnlyList<Snippet> usedSnippets,
            IReadOnlyList<MissingSnippet> missingSnippets,
            IReadOnlyList<Include> usedIncludes,
            IReadOnlyList<MissingInclude> missingIncludes,
            IReadOnlyList<ValidationError> validationErrors)
        {
            UsedSnippets = usedSnippets;
            UsedIncludes = usedIncludes;
            MissingIncludes = missingIncludes;
            MissingSnippets = missingSnippets;
            ValidationErrors = validationErrors;
        }

        /// <summary>
        /// List of all <see cref="Snippet"/>s that the markdown file used.
        /// </summary>
        public IReadOnlyList<Snippet> UsedSnippets { get; }

        /// <summary>
        /// List of all <see cref="Include"/>s that the markdown file used.
        /// </summary>
        public IReadOnlyList<Include> UsedIncludes { get; }

        /// <summary>
        /// Enumerates through the <see cref="UsedSnippets" /> but will first throw an exception if there are any <see cref="MissingSnippets" />.
        /// </summary>
        public virtual IEnumerator<Snippet> GetEnumerator()
        {
            if (MissingSnippets.Any())
            {
                throw new MissingSnippetsException(MissingSnippets);
            }

            if (MissingIncludes.Any())
            {
                throw new MissingIncludesException(MissingIncludes);
            }

            if (ValidationErrors.Any())
            {
                throw new ContentValidationException(ValidationErrors);
            }

            return UsedSnippets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// List of all snippets that the markdown file expected but did not exist in the input snippets.
        /// </summary>
        public IReadOnlyList<MissingSnippet> MissingSnippets { get; }

        /// <summary>
        /// List of all validation errors that the markdown file.
        /// </summary>
        public IReadOnlyList<ValidationError> ValidationErrors { get; }

        /// <summary>
        /// List of all includes that the markdown file expected but did not exist in the input includes.
        /// </summary>
        public IReadOnlyList<MissingInclude> MissingIncludes { get; }
    }
}