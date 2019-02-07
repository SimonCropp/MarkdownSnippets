using System;
using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{
    public class MissingSnippetsException :
        Exception
    {
        public MissingSnippetsException(IReadOnlyList<MissingSnippet> missing) :
            base($"Missing snippets: {string.Join(", ", missing.Select(x => x.Key))}")
        {
        }
    }
}