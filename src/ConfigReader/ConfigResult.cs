using System.Collections.Generic;
using MarkdownSnippets;

public class ConfigResult
{
    public bool ReadOnly;
    public LinkFormat LinkFormat;
    public int TocLevel;
    public int MaxWidth;
    public List<string> UrlsAsSnippets = new List<string>();
    public List<string> Exclude = new List<string>();
    public bool WriteHeader;
    public string? Header;
    public string? UrlPrefix;
    public List<string> TocExcludes = new List<string>();
    public bool TreatMissingSnippetAsWarning;
}