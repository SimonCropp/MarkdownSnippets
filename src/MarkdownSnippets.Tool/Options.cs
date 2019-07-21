﻿using System.Collections.Generic;
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

    [Option('r', "readonly", Required = false, HelpText = "Set resultant md files as read-only.")]
    public bool? ReadOnly { get; set; }

    [Option('h', "write-header", Required = false, HelpText = "Write a header at the top of each resultant md file.")]
    public bool? WriteHeader { get; set; }
}