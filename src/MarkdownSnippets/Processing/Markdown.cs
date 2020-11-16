using System.Text.RegularExpressions;

static class Markdown
{
    static Regex stripLinkRegex = new( @"\[(.*?)\][\[\(].*?[\]\)]",RegexOptions.Compiled);

    public static string StripMarkdown(string input)
    {
        var title = input.Replace("*","");
        return stripLinkRegex.Replace(title, "$1");
    }
}