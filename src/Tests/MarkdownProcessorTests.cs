using System.Collections.Generic;
using CaptureSnippets;
using Xunit;

public class MarkdownProcessorTests : TestBase
{
    [Fact]
    public void Simple()
    {
        var availableSnippets = new List<Snippet>
        {
            SnippetBuild(
                language: "cs",
                key: "snippet1",
                package: "package1"
            ),
            SnippetBuild(
                language: "cs",
                key: "snippet2",
                package: "package1"
            )
        };
        var markdownContent = @"
snippet: snippet1

some text

snippet: snippet2

some other text

";
        SnippetVerifier.Verify(markdownContent, availableSnippets);
    }

    Snippet SnippetBuild(string language, string key, string package)
    {
        return Snippet.Build(
            language: language,
            startLine: 1,
            endLine: 2,
            value: "Snippet",
            key: key,
            path: "thePath");
    }
}