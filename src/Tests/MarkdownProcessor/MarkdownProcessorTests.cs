using System.Collections.Generic;
using System.IO;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class MarkdownProcessorTests :
    XunitApprovalBase
{
    [Fact]
    public void WithInclude()
    {
        var content = @"
before

include: theKey

after
";
        SnippetVerifier.Verify(
            content,
            availableSnippets: new List<Snippet>(),
            snippetSourceFiles: new List<string>(),
            includes: new []{ Include.Build("theKey", new List<string> {"theValue"}, "thePath")});
    }

    [Fact]
    public void MissingInclude()
    {
        var content = @"
before

include: theKey

after
";
        SnippetVerifier.Verify(content,
            availableSnippets: new List<Snippet>(),
            snippetSourceFiles: new List<string>(),
            includes: new List<Include>());
    }

    [Fact]
    public void SkipHeadingBeforeToc()
    {
        var content = @"
## Heading 1

toc

Text1

## Heading 2

Text2

";
        SnippetVerifier.Verify(content, new List<Snippet>(), new List<string>());
    }

    [Fact]
    public void Toc()
    {
        var content = @"
# Title

toc

## Heading 1

Text1

## Heading 2

Text2

";
        SnippetVerifier.Verify(content, new List<Snippet>(), new List<string>());
    }

    [Fact]
    public void Simple()
    {
        var availableSnippets = new List<Snippet>
        {
            SnippetBuild(
                language: "cs",
                key: "snippet1"
            ),
            SnippetBuild(
                language: "cs",
                key: "snippet2"
            )
        };
        var content = @"
snippet: snippet1

some text

snippet: snippet2

some other text

snippet: FileToUseAsSnippet.txt

some other text

snippet: /FileToUseAsSnippet.txt

";
        SnippetVerifier.Verify(
            content,
            availableSnippets,
            new List<string>
            {
                Path.Combine(GitRepoDirectoryFinder.FindForFilePath(), "src/Tests/FileToUseAsSnippet.txt")
            });
    }

    static Snippet SnippetBuild(string language, string key)
    {
        return Snippet.Build(
            language: language,
            startLine: 1,
            endLine: 2,
            value: "Snippet",
            key: key,
            path: "thePath");
    }

    public MarkdownProcessorTests(ITestOutputHelper output) :
        base(output)
    {
    }
}