using MarkdownSnippets;

public class ConfigResult
{
    public bool? ReadOnly;
    public bool ValidateContent;
    public bool HashSnippetAnchors;
    public bool OmitSnippetLinks;
    public LinkFormat LinkFormat;
    public DocumentConvention Convention;
    public int TocLevel;
    public int MaxWidth;
    public List<string> UrlsAsSnippets = new();
    public List<string> ExcludeDirectories = new();
    public List<string> ExcludeMarkdownDirectories = new();
    public List<string> ExcludeSnippetDirectories = new();
    public bool? WriteHeader;
    public string? Header;
    public string? UrlPrefix;
    public List<string> TocExcludes = new();
    public bool TreatMissingAsWarning;
}