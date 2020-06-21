using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VerifyXunit;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

[UsesVerify]
public class SnippetMarkdownHandlingTests
{
    [Fact]
    public Task AppendGroup()
    {
        var builder = new StringBuilder();
        var snippets = new List<Snippet> {Snippet.Build(1, 2, "theValue", "thekey", "thelanguage", "c:/dir/thePath")};
        var markdownHandling = new SnippetMarkdownHandling("c:/dir/", LinkFormat.GitHub);
        using (var writer = new StringWriter(builder))
        {
            markdownHandling.AppendGroup("key1", snippets, writer.WriteLine);
        }

        return Verifier.Verify(builder.ToString());
    }
}