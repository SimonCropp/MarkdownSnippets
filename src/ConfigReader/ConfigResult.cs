public class ConfigResult
{
    public bool? ReadOnly;
    public bool ValidateContent;
    public bool OmitSnippetLinks;
    public LinkFormat LinkFormat;
    public DocumentConvention Convention;
    public int TocLevel;
    public int MaxWidth;
    public List<string> UrlsAsSnippets = [];
    public List<string> ExcludeDirectories = [];
    public List<string> ExcludeMarkdownDirectories = [];
    public List<string> ExcludeSnippetDirectories = [];
    public bool? WriteHeader;
    public string? Header;
    public string? UrlPrefix;
    public List<string> TocExcludes = [];
    public bool TreatMissingAsWarning;
}