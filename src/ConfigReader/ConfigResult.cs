using System.Collections.Generic;
using MarkdownSnippets;

public class ConfigResult
{
    public bool? ReadOnly;
    public bool ValidateContent;
    public LinkFormat LinkFormat;
    public DocumentConvention Convention;
    public int TocLevel;
    public int MaxWidth;
    public List<string> UrlsAsSnippets = new List<string>();
    public List<string> Exclude = new List<string>();
    public bool? WriteHeader;
    public string? Header;
    public string? UrlPrefix;
    public List<string> TocExcludes = new List<string>();
    public List<string> DocumentExtensions = new List<string>();
    public bool TreatMissingAsWarning;
    public bool WritePath = true;
}