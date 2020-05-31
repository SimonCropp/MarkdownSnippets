using System.Collections.Generic;

public class ConfigSerialization
{
    public bool? ReadOnly { get; set; }
    public bool? ValidateContent { get; set; }
    public string? LinkFormat { get; set; }
    public bool? WriteHeader { get; set; }
    public string? Header { get; set; }
    public string? UrlPrefix { get; set; }
    public int? TocLevel { get; set; }
    public int? MaxWidth { get; set; }
    public List<string> UrlsAsSnippets { get; set; } = new List<string>();
    public List<string> Exclude { get; set; } = new List<string>();
    public List<string> TocExcludes { get; set; } = new List<string>();
    public bool? TreatMissingSnippetAsWarning { get; set; }
    public bool? TreatMissingIncludeAsWarning { get; set; }
}