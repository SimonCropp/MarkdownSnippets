using System.Collections.Generic;

public class ConfigSerialization
{
    public bool? WriteHeader { get; set; }
    public bool? ReadOnly { get; set; }
    public string LinkFormat { get; set; }
    public List<string> UrlsAsSnippets { get; set; } = new List<string>();
    public List<string> Exclude { get; set; } = new List<string>();
}