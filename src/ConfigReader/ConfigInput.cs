using System.Collections.Generic;
using MarkdownSnippets;

public class ConfigInput
{
    public bool? ReadOnly { get; set; }
    public LinkFormat? LinkFormat { get; set; }
    public int? TocLevel { get; set; }
    public int? MaxWidth { get; set; }
    public List<string> UrlsAsSnippets { get; set; } = new List<string>();
    public List<string> Exclude { get; set; } = new List<string>();
    public bool? WriteHeader { get; set; }
    public string? Header { get; set; }
    public string? UrlPrefix { get; set; }
    public List<string> TocExcludes { get; set; } = new List<string>();
    public bool? TreatMissingSnippetAsWarning { get; set; }
    public bool? TreatMissingIncludeAsWarning { get; set; }
}