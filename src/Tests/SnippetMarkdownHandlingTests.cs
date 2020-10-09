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
        var markdownHandling = new SnippetMarkdownHandling("c:/dir/", LinkFormat.GitHub, false);
        using (var writer = new StringWriter(builder))
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
        var markdownHandling = new SnippetMarkdownHandling("c:/dir/", LinkFormat.GitHub, false, "prefix-");
        using (var writer = new StringWriter(builder))
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
        var markdownHandling = new SnippetMarkdownHandling("c:/dir/", LinkFormat.GitHub, true);
        using (var writer = new StringWriter(builder))
        {
            markdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verifier.Verify(builder.ToString());
    }

    static List<Snippet> Snippets()
    {
        return new List<Snippet>
        {
            Snippet.Build(1, 2, "theValue", "thekey", "thelanguage", "c:/dir/thePath")
        };
    }
}