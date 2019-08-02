using System.Collections.Generic;
using MarkdownSnippets;

public class ConfigResult
{
    public bool ReadOnly;
    public LinkFormat LinkFormat;
    public int TocLevel;
    public List<string> UrlsAsSnippets = new List<string>();
    public List<string> Exclude = new List<string>();
    public bool WriteHeader;
    public string Header;
    public List<string> TocExcludes = new List<string>();
}