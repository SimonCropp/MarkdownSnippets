using System.Collections.Generic;
using MarkdownSnippets;

public delegate void Invoke(string targetDirectory, bool? readOnly, bool? writeHeader, LinkFormat? linkFormat, List<string> exclude);