using System.Collections.Generic;
using MarkdownSnippets;

public class ConfigResult
{
    public bool ReadOnly;
    public bool WriteHeader;
    public LinkFormat LinkFormat;
    public List<string> UrlsAsSnippets = new List<string>();
    public List<string> Exclude = new List<string>();
}