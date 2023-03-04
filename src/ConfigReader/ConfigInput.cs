using MarkdownSnippets;

public class ConfigInput
{
    public bool? ReadOnly { get; init; }
    public bool? ValidateContent { get; init; }
    public bool? OmitSnippetLinks { get; init; }
    public LinkFormat? LinkFormat { get; init; }
    public DocumentConvention? Convention { get; init; }
    public int? TocLevel { get; init; }
    public int? MaxWidth { get; init; }
    public List<string> UrlsAsSnippets { get; init; } = new();
    public List<string> ExcludeDirectories { get; init; } = new();
    public List<string> ExcludeMarkdownDirectories { get; init; } = new();
    public List<string> ExcludeSnippetDirectories { get; init; } = new();
    public bool? WriteHeader { get; init; }
    public string? Header { get; init; }
    public string? UrlPrefix { get; init; }
    public List<string> TocExcludes { get; init; } = new();
    public bool? TreatMissingAsWarning { get; init; }
}