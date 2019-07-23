using System.Collections.Generic;
using MarkdownSnippets;

public class ConfigInput
{
    public bool? WriteHeader { get; set; }
    public bool? ReadOnly { get; set; }
    public LinkFormat? LinkFormat { get; set; }
    public List<string> UrlsAsSnippets { get; set; } = new List<string>();
    public List<string> Exclude { get; set; } = new List<string>();
}