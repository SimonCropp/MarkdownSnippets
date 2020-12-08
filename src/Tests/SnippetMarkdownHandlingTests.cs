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
        StringBuilder builder = new();
        var snippets = Snippets();
        SnippetMarkdownHandling markdownHandling = new("c:/dir/", LinkFormat.GitHub, false);
        using (StringWriter writer = new(builder))
        {
            markdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verifier.Verify(builder.ToString());
    }

    [Fact]
    public Task AppendPrefixed()
    {
        StringBuilder builder = new();
        var snippets = Snippets();
        SnippetMarkdownHandling markdownHandling = new("c:/dir/", LinkFormat.GitHub, false, "prefix-");
        using (StringWriter writer = new(builder))
        {
            markdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verifier.Verify(builder.ToString());
    }

    [Fact]
    public Task AppendHashed()
    {
        StringBuilder builder = new();
        var snippets = Snippets();
        SnippetMarkdownHandling markdownHandling = new("c:/dir/", LinkFormat.GitHub, true);
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