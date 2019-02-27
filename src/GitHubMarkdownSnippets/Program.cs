using System;

class Program
{
    static void Main()
    {
        Console.WriteLine(@"Obsolete. Use MarkdownSnippets.Tool instead.
Use the following to change over:
dotnet tool uninstall -g GitHubMarkdownSnippets
dotnet tool install -g MarkdownSnippets.Tool");
        Environment.Exit(1);
    }
}