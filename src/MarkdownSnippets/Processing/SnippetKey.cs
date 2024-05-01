using System.Diagnostics.CodeAnalysis;

class NewSnippet:ISnippetPart
{
    public bool ShouldExcludeFromIncludeProcessing(string lineCurrent) =>
        lineCurrent.StartsWith("snippet:", StringComparison.OrdinalIgnoreCase);


    public string Handle(MarkdownProcessor markdownProcessor, List<Line> lines, string? relativePath, StringBuilder builder, int index, List<MissingSnippet> missingSnippets, List<Snippet> usedSnippets, Action<string> appendLine, string key, Line line)
    {
        return markdownProcessor.ProcessSnippetLine(missingSnippets, usedSnippets, key, relativePath, line);
    }
    public bool ExtractSnippet(Line line, [NotNullWhen(true)] out string? key)
    {
        var lineCurrent = line.Current;
        if (!ShouldExcludeFromIncludeProcessing(lineCurrent))
        {
            key = null;
            return false;
        }

        key = line.Current[8..]
            .Trim();
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new SnippetException($"Could not parse snippet from: {line.Original}. Path: {line.Path}. Line: {line.LineNumber}");
        }
        return true;
    }
}

public interface ISnippet
{
    public ISnippetPart GetNew { get;  }
    public ISnippetPart GetReplace { get; }
}
public interface ISnippetPart
{
    string Handle(MarkdownProcessor markdownProcessor, List<Line> lines, string? relativePath, StringBuilder builder, int index, List<MissingSnippet> missingSnippets, List<Snippet> usedSnippets, Action<string> appendLine, string key, Line line);
    bool ExtractSnippet(Line line, [NotNullWhen(true)] out string? key);
    public bool ShouldExcludeFromIncludeProcessing(string line);
}
class CodeSnippet:ISnippet
{
    public ISnippetPart GetNew { get; } = new NewSnippet();
    public ISnippetPart GetReplace { get; } = new ReplaceSnippet();
}
class ReplaceSnippet : ISnippetPart
{
    public bool ShouldExcludeFromIncludeProcessing(string lineCurrent) =>
        lineCurrent.StartsWith("<!-- snippet:", StringComparison.OrdinalIgnoreCase);
    public string Handle(MarkdownProcessor markdownProcessor, List<Line> lines, string? relativePath, StringBuilder builder, int index, List<MissingSnippet> missingSnippets, List<Snippet> usedSnippets, Action<string> appendLine, string key, Line line)
    {
        builder.Clear();
        appendLine(markdownProcessor.ProcessSnippetLine( missingSnippets, usedSnippets, key, relativePath, line));
        builder.TrimEnd();

        lines.RemoveUntil(
            index+1,
            "<!-- endSnippet -->",
            relativePath,
            line);
        return builder.ToString();
    }
    public bool ExtractSnippet(Line line, [NotNullWhen(true)] out string? key)
    {
        var lineCurrent = line.Current;
        if (!ShouldExcludeFromIncludeProcessing(lineCurrent))
        {
            key = null;
            return false;
        }

        var substring = line.Current[14..];
        var indexOf = substring.IndexOf("-->");
        key = substring[..indexOf]
            .Trim();
        return true;
    }
}