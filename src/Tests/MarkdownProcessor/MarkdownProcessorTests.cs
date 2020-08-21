using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MarkdownSnippets;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class MarkdownProcessorTests
{
    [Fact]
    public Task Missing_endInclude()
    {
        var content = @"
BAD <!-- include: theKey. path: /thePath -->
";
        return SnippetVerifier.VerifyThrows<MarkdownProcessingException>(
            DocumentConvention.InPlaceOverwrite,
            content,
            includes: new[]
            {
                Include.Build("theKey", Array.Empty<string>(), "c:/root/thePath")
            });
    }

    [Fact]
    public Task WithEmptyMultiLineInclude_Overwrite()
    {
        var content = @"
before

 <!-- include: theKey. path: /thePath -->

 <!-- endInclude -->

after
";
        var lines = new List<string> {"one", "two"};
        return SnippetVerifier.Verify(
            DocumentConvention.InPlaceOverwrite,
            content,
            includes: new[]
            {
                Include.Build("theKey", lines, "c:/root/thePath")
            });
    }
    [Fact]
    public Task WithMultiLineInclude_Overwrite()
    {
        var content = @"
before

BAD <!-- include: theKey. path: /thePath -->
BAD
BAD <!-- endInclude -->

after
";
        var lines = new List<string> {"theValue1", "theValue2"};
        return SnippetVerifier.Verify(
            DocumentConvention.InPlaceOverwrite,
            content,
            includes: new[]
            {
                Include.Build("theKey", lines, "c:/root/thePath")
            });
    }

    [Fact]
    public Task WithSingleInclude_Overwrite()
    {
        var content = @"
before

BAD <!-- singleLineInclude: theKey. path: /thePath -->

after
";
        var lines = new List<string> {"theValue1"};
        return SnippetVerifier.Verify(
            DocumentConvention.InPlaceOverwrite,
            content,
            includes: new[]
            {
                Include.Build("theKey", lines, "c:/root/thePath")
            });
    }

    [Fact]
    public Task WithSingleInclude()
    {
        var content = @"
before

include: theKey

after
";
        var lines = new List<string> {"theValue1"};
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            includes: new[]
            {
                Include.Build("theKey", lines, "c:/root/thePath")
            });
    }

    [Fact]
    public Task WithDoubleInclude()
    {
        var content = @"
before

include: theKey

after
";
        var lines = new List<string> {"theValue1", "theValue2"};
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            includes: new[]
            {
                Include.Build("theKey", lines, "c:/root/thePath")
            });
    }

    [Fact]
    public Task WithEmptyMultipleInclude()
    {
        var content = @"
before

include: theKey

after
";
        var lines = new List<string> {"", "", ""};
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            includes: new[]
            {
                Include.Build("theKey", lines, "c:/root/thePath")
            });
    }
    [Fact]
    public Task WithMultipleInclude()
    {
        var content = @"
before

include: theKey

after
";
        var lines = new List<string> {"theValue1", "theValue2", "theValue3"};
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            includes: new[]
            {
                Include.Build("theKey", lines, "c:/root/thePath")
            });
    }

    [Fact]
    public Task MissingInclude()
    {
        var content = @"
before

include: theKey

after
";
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content);
    }

    [Fact]
    public Task SkipHeadingBeforeToc()
    {
        var content = @"
## Heading 1

toc

Text1

## Heading 2

Text2
";
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content);
    }

    [Fact]
    public Task Toc1()
    {
        var content = @"
# Title

toc1

## Heading 1

Text1

## Heading 2

Text2
";
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content);
    }

    [Fact]
    public Task Toc()
    {
        var content = @"
# Title

toc

## Heading 1

Text1

## Heading 2

Text2
";
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content);
    }

    [Fact]
    public Task Missing_endToc()
    {
        var content = @"
<!-- toc -->
Bad
";
        return SnippetVerifier.VerifyThrows<MarkdownProcessingException>(
            DocumentConvention.InPlaceOverwrite,
            content);
    }
    [Fact]
    public Task Toc_Overwrite()
    {
        var content = @"
# Title

<!-- toc -->
Bad<!-- endToc -->

## Heading 1

Text1

## Heading 2

Text2
";
        return SnippetVerifier.Verify(
            DocumentConvention.InPlaceOverwrite,
            content);
    }

    [Fact]
    public Task Simple_Overwrite()
    {
        var availableSnippets = new List<Snippet>
        {
            SnippetBuild("cs", "snippet1"),
            SnippetBuild("cs", "snippet2"
            )
        };
        var content = @"
<!-- snippet: snippet1 -->
```cs
BAD
```
<!-- endSnippet -->

some text

<!-- snippet: snippet2 -->
```cs
BAD
```
<!-- endSnippet -->

some other text

<!-- snippet: FileToUseAsSnippet.txt -->
```txt
BAD
```
<!-- endSnippet -->

some other text

<!-- snippet: /FileToUseAsSnippet.txt -->
```txt
BAD
```
<!-- endSnippet -->
";
        return SnippetVerifier.Verify(
            DocumentConvention.InPlaceOverwrite,
            content,
            availableSnippets,
            new List<string>
            {
                Path.Combine(GitRepoDirectoryFinder.FindForFilePath(), "src/Tests/FileToUseAsSnippet.txt")
            });
    }

    [Fact]
    public Task Simple()
    {
        var availableSnippets = new List<Snippet>
        {
            SnippetBuild("cs", "snippet1"),
            SnippetBuild("cs", "snippet2")
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
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            availableSnippets,
            new List<string>
            {
                Path.Combine(GitRepoDirectoryFinder.FindForFilePath(), "src/Tests/FileToUseAsSnippet.txt")
            });
    }

    [Fact]
    public Task SnippetInInclude()
    {
        var availableSnippets = new List<Snippet>
        {
            SnippetBuild("cs", "snippet1")
        };
        var content = @"
some text

include: theKey

some other text
";
        var lines = new List<string> {"snippet: snippet1"};
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            availableSnippets,
            includes: new[]
            {
                Include.Build("theKey", lines, "thePath")
            });
    }

    [Fact]
    public Task SnippetInIncludeLast()
    {
        var availableSnippets = new List<Snippet>
        {
            SnippetBuild("cs", "snippet1")
        };
        var content = @"
some text

include: theKey

some other text
";
        var lines = new List<string> {"line1", "snippet: snippet1"};
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            availableSnippets,
            includes: new[]
            {
                Include.Build("theKey", lines, "thePath")
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
}