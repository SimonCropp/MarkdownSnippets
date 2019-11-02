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
            IReadOnlyList<Include> usedIncludes)
        {
            Guard.AgainstNull(usedSnippets, nameof(usedSnippets));
            Guard.AgainstNull(missingSnippets, nameof(missingSnippets));
            Guard.AgainstNull(usedIncludes, nameof(usedIncludes));
            UsedSnippets = usedSnippets;
            UsedIncludes = usedIncludes;
            MissingSnippets = missingSnippets;
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

            return UsedSnippets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// List of all snippets that the markdown file expected but did not exist in the input snippets.
        /// </summary>
        public IReadOnlyList<MissingSnippet> MissingSnippets { get; }
    }
}