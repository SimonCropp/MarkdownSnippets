using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VerifyXunit;
using MarkdownSnippets;
using Xunit;

[UsesVerify]
public class SimpleSnippetMarkdownHandlingTests
{
    [Fact]
    public Task Append()
    {
        var builder = new StringBuilder();
        var snippets = new List<Snippet> {Snippet.Build(1, 2, "theValue", "thekey", "thelanguage", "thePath")};
        using (var writer = new StringWriter(builder))
        {
            SimpleSnippetMarkdownHandling.Append("key1", snippets, writer.WriteLine);
        }

        return Verifier.Verify(builder.ToString());
    }
}