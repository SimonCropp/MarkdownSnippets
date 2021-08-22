using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VerifyXunit;
using MarkdownSnippets;
using Xunit;

[UsesVerify]
public class SnippetMarkdownHandlingTests
{
    [Fact]
    public Task Append()
    {
        var builder = new StringBuilder();
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling("c:/dir/", LinkFormat.GitHub, false,false);
        using (StringWriter writer = new(builder))
        {
            markdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verifier.Verify(builder.ToString());
    }
    [Fact]
    public Task AppendOmitSnippetLinks()
    {
        var builder = new StringBuilder();
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling("c:/dir/", LinkFormat.GitHub, false,true);
        using (StringWriter writer = new(builder))
        {
            markdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verifier.Verify(builder.ToString());
    }

    [Fact]
    public Task AppendPrefixed()
    {
        var builder = new StringBuilder();
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling("c:/dir/", LinkFormat.GitHub, false,false, "prefix-");
        using (StringWriter writer = new(builder))
        {
            markdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verifier.Verify(builder.ToString());
    }

    [Fact]
    public Task AppendHashed()
    {
        var builder = new StringBuilder();
        var snippets = Snippets();
        var markdownHandling = new SnippetMarkdownHandling("c:/dir/", LinkFormat.GitHub,false, true);
        using (StringWriter writer = new(builder))
        {
            markdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verifier.Verify(builder.ToString());
    }

    static List<Snippet> Snippets()
    {
        return new()
        {
            Snippet.Build(1, 2, "theValue", "thekey", "thelanguage", "c:/dir/thePath")
        };
    }
}