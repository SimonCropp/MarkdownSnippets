using CommandLine;

public class Options
{
    [Option('t', "target-directory", Required = false)]
    public string TargetDirectory { get; set; }
}