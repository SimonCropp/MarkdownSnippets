using System.Collections.Generic;
using CommandLine;
using MarkdownSnippets;

public class Options
{
    [Option('t', "target-directory",
        Required = false,
        HelpText = @"The target directory to run against. Optional. If no directory is passed the current directory will be used, but only if it exists with a git repository directory tree. If not an error is returned.")]
    public string TargetDirectory { get; set; }

    [Option('e', "exclude",
        Separator = ':',
        Required = false,
        HelpText = "Directories to be excluded. Optional. Colon ':' separated for multiple values.")]
    public IList<string> Exclude { get; set; }

    [Option("toc-excludes",
        Separator = ':',
        Required = false,
        HelpText = "Headings to be excluded from table of contents. Optional. Colon ':' separated for multiple values.")]
    public IList<string> TocExcludes { get; set; }

    [Option('u', "urls-as-snippets",
        Separator = ':',
        Required = false,
        HelpText = "UrlsAsSnippets to be included as snippets. Optional. Colon ':' separated for multiple values.")]
    public IList<string> UrlsAsSnippets { get; set; }

    [Option('r', "readonly",
        Required = false,
        HelpText = "Set resultant md files as read-only. Optional. Defaults to false.")]
    public bool? ReadOnly { get; set; }

    [Option('h', "write-header",
        Required = false,
        HelpText = "Write a header at the top of each resultant md file. Optional. Defaults to true")]
    public bool? WriteHeader { get; set; }

    [Option('l', "link-format",
        Required = false,
        HelpText = "Controls the format of the link under each snippet. Optional. Supported values: GitHub, Tfs. Defaults to GitHub.")]
    public LinkFormat? LinkFormat { get; set; }

    [Option("toc-level",
        Required = false,
        HelpText = "Controls how many header levels to write in the table of contents. Optional. Defaults to 2. Must be positive.")]
    public int? TocLevel { get; set; }
}