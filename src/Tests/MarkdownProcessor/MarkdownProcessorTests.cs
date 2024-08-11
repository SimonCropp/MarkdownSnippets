public class MarkdownProcessorTests
{
    [Fact]
    public Task Missing_endInclude()
    {
        var content = """

                      BAD<!-- include: theKey. path: /thePath -->

                      """;
        return SnippetVerifier.VerifyThrows(
            DocumentConvention.InPlaceOverwrite,
            content,
            includes: [Include.Build("theKey", [], Path.GetFullPath("thePath"))]);
    }

    [Fact]
    public Task WithEmptyMultiLineInclude_Overwrite()
    {
        var content = """

                      before

                      <!-- include: theKey. path: /thePath -->

                      <!-- endInclude -->

                      after

                      """;
        var lines = new List<string>
        {
            "one",
            "two"
        };
        return SnippetVerifier.Verify(
            DocumentConvention.InPlaceOverwrite,
            content,
            includes: [Include.Build("theKey", lines, Path.GetFullPath("thePath"))]);
    }

    [Fact]
    public Task WithMultiLineInclude_Overwrite()
    {
        var content = """

                      before

                      BAD<!-- include: theKey. path: /thePath -->
                      BAD
                      BAD<!-- endInclude -->

                      after

                      """;
        var lines = new List<string>
        {
            "theValue1",
            "theValue2"
        };
        return SnippetVerifier.Verify(
            DocumentConvention.InPlaceOverwrite,
            content,
            includes: [Include.Build("theKey", lines, Path.GetFullPath("thePath"))]);
    }

    [Fact]
    public Task WithSingleInclude_Overwrite()
    {
        var content = """

                      before

                      BAD<!-- singleLineInclude: theKey. path: /thePath -->

                      after

                      """;
        var lines = new List<string>
        {
            "theValue1"
        };
        return SnippetVerifier.Verify(
            DocumentConvention.InPlaceOverwrite,
            content,
            includes: [Include.Build("theKey", lines, Path.GetFullPath("thePath"))]);
    }

    [Fact]
    public Task WithSingleInclude()
    {
        var content = """

                      before

                      include: theKey

                      after

                      """;
        var lines = new List<string>
        {
            "theValue1"
        };
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            includes: [Include.Build("theKey", lines, Path.GetFullPath("thePath"))]);
    }

    [Fact]
    public Task WithMixedCaseInclude()
    {
        var content = """

                      before

                      include: theKey

                      include: TheKey

                      after

                      """;
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            includes:
            [
                Include.Build("theKey", ["theValue1"], Path.GetFullPath("thePath")),
                Include.Build("TheKey", ["theValue2"], Path.GetFullPath("thePath"))
            ]);
    }

    [Fact]
    public Task WithSingleSnippet()
    {
        var content = """

                      before

                      snippet: theKey

                      after

                      """;

        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            snippets: [SnippetBuild("cs", "theKey")]);
    }
    [Fact]
    public Task WithMixedCaseSnippet()
    {
        var content = """

                      before

                      snippet: theKey

                      snippet: TheKey

                      after

                      """;

        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            snippets:
            [
                SnippetBuild("cs", "theKey"),
                SnippetBuild("cs", "TheKey"),
            ]);
    }

    [Fact]
    public Task WithTwoLineSnippet()
    {
        var content = """

                      before

                      snippet: theKey

                      after

                      """;

        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            snippets:
            [
                Snippet.Build(
                    language: "cs",
                    startLine: 1,
                    endLine: 2,
                    value: """
                           the
                           Snippet
                           """,
                    key: "theKey",
                    path: "thePath")
            ]);
    }

    [Fact]
    public Task WithMultiLineSnippet()
    {
        var content = """

                      before

                      snippet: theKey

                      after

                      """;

        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            snippets:
            [
                Snippet.Build(
                    language: "cs",
                    startLine: 1,
                    endLine: 2,
                    value: """
                           the
                           long
                           Snippet
                           """,
                    key: "theKey",
                    path: "thePath")
            ]);
    }

    [Fact]
    public Task WithDoubleInclude()
    {
        var content = """

                      before

                      include: theKey

                      after

                      """;
        var lines = new[]
        {
            "theValue1",
            "theValue2"
        };
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            includes:
            [
                Include.Build("theKey", lines, Path.GetFullPath("thePath"))
            ]);
    }

    [Fact]
    public Task WithEmptyMultipleInclude()
    {
        var content = """

                      before

                      include: theKey

                      after

                      """;
        var lines = new[]
        {
            "",
            "",
            ""
        };
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            includes: [Include.Build("theKey", lines, Path.GetFullPath("thePath"))]);
    }

    [Fact]
    public Task WithMultipleInclude()
    {
        var content = """

                      before

                      include: theKey

                      after

                      """;
        var lines = new[]
        {
            "theValue1",
            "theValue2",
            "theValue3"
        };
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            includes: [Include.Build("theKey", lines, Path.GetFullPath("thePath"))]);
    }

    [Fact]
    public Task MissingInclude()
    {
        var content = """

                      before

                      include: theKey

                      after

                      """;
        return SnippetVerifier.Verify(DocumentConvention.SourceTransform, content);
    }

    [Fact]
    public Task SkipHeadingBeforeToc()
    {
        var content = """

                      ## Heading 1

                      toc

                      Text1

                      ## Heading 2

                      Text2

                      """;
        return SnippetVerifier.Verify(DocumentConvention.SourceTransform, content);
    }

    [Fact]
    public Task Toc1()
    {
        var content = """

                      # Title

                      toc1

                      ## Heading 1

                      Text1

                      ## Heading 2

                      Text2

                      """;
        return SnippetVerifier.Verify(DocumentConvention.SourceTransform, content);
    }

    [Fact]
    public Task Toc()
    {
        var content = """

                      # Title

                      toc

                      ## Heading 1

                      Text1

                      ## Heading 2

                      Text2

                      """;
        return SnippetVerifier.Verify(DocumentConvention.SourceTransform, content);
    }

    [Fact]
    public Task TocRetainedIfNoHeadingsInFile()
    {
        var content = """

                      # Title

                      toc

                      This document has no headings.

                      An empty toc section should be generated, in case
                      any headings are added in future.

                      """;
        return SnippetVerifier.Verify(DocumentConvention.SourceTransform, content);
    }

    [Fact]
    public Task Missing_endToc()
    {
        var content = """

                      <!-- toc -->
                      Bad

                      """;
        return SnippetVerifier.VerifyThrows(DocumentConvention.InPlaceOverwrite, content);
    }

    [Fact]
    public Task Empty_snippet_key()
    {
        var content = """

                      snippet:


                      """;
        return SnippetVerifier.VerifyThrows(DocumentConvention.InPlaceOverwrite, content);
    }

    [Fact]
    public Task Whitespace_snippet_key()
    {
        var content = """

                      snippet:


                      """;
        return SnippetVerifier.VerifyThrows(DocumentConvention.InPlaceOverwrite, content);
    }

    [Fact]
    public Task Toc_Overwrite()
    {
        var content = """

                      # Title

                      <!-- toc -->
                      Bad<!-- endToc -->

                      ## Heading 1

                      Text1

                      ## Heading 2

                      Text2

                      """;
        return SnippetVerifier.Verify(DocumentConvention.InPlaceOverwrite, content);
    }

    [Fact]
    public Task Simple_Overwrite()
    {
        var availableSnippets = new List<Snippet>
        {
            SnippetBuild("cs", "snippet1"),
            SnippetBuild("cs", "snippet2")
        };
        var content = """

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

                      """;
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
    public async Task MixedNewlinesInFile()
    {
        var file = "FileWithMixedNewLines.txt";
        File.Delete(file);
        File.WriteAllText(file, "a\rb\nc\r\nd");
        var availableSnippets = new List<Snippet>();
        var content = """

                      some other text

                      snippet: FileWithMixedNewLines.txt

                      """;
        var result = await SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            availableSnippets,
            new List<string>
            {
                file
            });

        Assert.DoesNotContain("\r\n", result);
        Assert.DoesNotContain("\r", result);
    }

    [Fact]
    public Task Simple()
    {
        var availableSnippets = new List<Snippet>
        {
            SnippetBuild("cs", "snippet1"),
            SnippetBuild("cs", "snippet2")
        };
        var content = """

                      snippet: snippet1

                      some text

                      snippet: snippet2

                      some other text

                      snippet: FileToUseAsSnippet.txt

                      some other text

                      snippet: /FileToUseAsSnippet.txt

                      """;
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
        var content = """

                      some text

                      include: theKey

                      some other text

                      """;
        var lines = new List<string>
        {
            "snippet: snippet1"
        };
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            availableSnippets,
            includes: [Include.Build("theKey", lines, "thePath")]);
    }

    [Fact]
    public Task TableInInclude()
    {
        var availableSnippets = new List<Snippet>();
        var content = """

                      some text

                      include: theKey

                      some other text

                      """;
        var lines = new List<string>
        {
            """
            | Number of Parameters | Variations per Parameter | Total Combinations | Pairwise Combinations |
            | -------------------- | ----------------------- | ------------------ | --------------------- |
            |2|5|25|25|
            """
        };
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            availableSnippets,
            includes: [Include.Build("theKey", lines, "thePath")]);
    }

    [Fact]
    public Task SnippetInIncludeLast()
    {
        var availableSnippets = new List<Snippet>
        {
            SnippetBuild("cs", "snippet1")
        };
        var content = """

                      some text

                      include: theKey

                      some other text

                      """;
        var lines = new List<string>
        {
            "line1",
            "snippet: snippet1"
        };
        return SnippetVerifier.Verify(
            DocumentConvention.SourceTransform,
            content,
            availableSnippets,
            includes: [Include.Build("theKey", lines, "thePath")]);
    }

    static Snippet SnippetBuild(string language, string key) =>
        Snippet.Build(
            language: language,
            startLine: 1,
            endLine: 2,
            value: "Snippet",
            key: key,
            path: "thePath");
}