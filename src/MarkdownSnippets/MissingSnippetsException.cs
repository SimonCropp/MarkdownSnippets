using System;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownSnippets
{
    public class MissingSnippetsException :
        Exception
    {
        public IReadOnlyList<MissingSnippet> Missing { get; }

        public MissingSnippetsException(IReadOnlyList<MissingSnippet> missing) :
            base($"Missing snippets: {string.Join(", ", missing.Select(x => x.Key))}")
        {
            Missing = missing;
        }
    }
}