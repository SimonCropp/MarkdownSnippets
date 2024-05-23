class CodeSnippet:ISnippet
{
    public ISnippetPart GetNew { get; } = new NewSnippet();
    public ISnippetPart GetReplace { get; } = new ReplaceSnippet();
}