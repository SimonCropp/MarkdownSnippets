using System.Collections.Generic;
using CommandLine;

public class Options
{
    [Option('t', "target-directory", Required = false)]
    public string TargetDirectory { get; set; }

    [Option('e', "exclude",
        Separator = ':',
        Required = false,
        HelpText = "Directories to be excluded")]
    public IList<string> Exclude { get; set; }

    [Option('r', "readonly", Required = false, HelpText = "Set output to verbose messages.")]
    public bool ReadOnly { get; set; }
}