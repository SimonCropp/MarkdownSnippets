public class MarkdownProcessorTests
{
    [Fact]
    public Task Missing_endInclude()
    {
        var content = """

                      BAD<!-- include: theKey. path: /thePath -->

                      """;
        IReadOnlyList<Include> includes = [Include.Build("theKey", [], Path.GetFullPath("thePath"))];
        var processor = SnippetVerifier.BuildProcessor(DocumentConvention.InPlaceOverwrite, null, null, includes);

        return Throws(() => SnippetVerifier.Apply(content, processor))
            .Snapshot(
                """
                {
                  Type: MarkdownProcessingException,
                  LineNumber: 2,
                  Message: Expected to find `<!-- endInclude -->`. File: . LineNumber: 2.
                }
                """);
    }

    [Fact]
    public async Task WithEmptyMultiLineInclude_Overwrite()
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
        IReadOnlyList<Include>? includes = [Include.Build("theKey", lines, Path.GetFullPath("thePath"))];
        var output = SnippetVerifier.Render(DocumentConvention.InPlaceOverwrite, content, null, null, includes);
        await Verify(output)
            .Snapshot(
                """
                {
                  result:
                before

                one<!-- include: theKey. path: {CurrentDirectory}thePath -->
                two<!-- endInclude -->

                after
                }
                """);
    }

    [Fact]
    public async Task WithMultiLineInclude_Overwrite()
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
        IReadOnlyList<Include>? includes = [Include.Build("theKey", lines, Path.GetFullPath("thePath"))];
        var output = SnippetVerifier.Render(DocumentConvention.InPlaceOverwrite, content, null, null, includes);
        await Verify(output)
            .Snapshot(
                """
                {
                  result:
                before

                theValue1<!-- include: theKey. path: {CurrentDirectory}thePath -->
                theValue2<!-- endInclude -->

                after
                }
                """);
    }

    [Fact]
    public async Task WithSingleInclude_Overwrite()
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
        IReadOnlyList<Include>? includes = [Include.Build("theKey", lines, Path.GetFullPath("thePath"))];
        var output = SnippetVerifier.Render(DocumentConvention.InPlaceOverwrite, content, null, null, includes);
        await Verify(output)
            .Snapshot(
                """
                {
                  result:
                before

                theValue1<!-- singleLineInclude: theKey. path: {CurrentDirectory}thePath -->

                after
                }
                """);
    }

    [Fact]
    public async Task WithSingleInclude()
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
        IReadOnlyList<Include>? includes = [Include.Build("theKey", lines, Path.GetFullPath("thePath"))];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, null, null, includes);
        await Verify(output)
            .Snapshot(
                """
                {
                  result:
                before

                theValue1<!-- singleLineInclude: theKey. path: {CurrentDirectory}thePath -->

                after
                }
                """);
    }

    [Fact]
    public async Task WithMixedCaseInclude()
    {
        var content = """

                      before

                      include: theKey

                      include: TheKey

                      after

                      """;
        IReadOnlyList<Include>? includes =
        [
            Include.Build("theKey", ["theValue1"], Path.GetFullPath("thePath")),
            Include.Build("TheKey", ["theValue2"], Path.GetFullPath("thePath"))
        ];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, null, null, includes);
        await Verify(output)
            .Snapshot(
                """
                {
                  result:
                before

                theValue1<!-- singleLineInclude: theKey. path: {CurrentDirectory}thePath -->

                theValue2<!-- singleLineInclude: TheKey. path: {CurrentDirectory}thePath -->

                after
                }
                """);
    }

    [Fact]
    public async Task WithSingleSnippet()
    {
        var content = """

                      before

                      snippet: theKey

                      after

                      """;

        List<Snippet>? snippets = [SnippetBuild("cs", "theKey")];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, snippets, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task WithMixedCaseSnippet()
    {
        var content = """

                      before

                      snippet: theKey

                      snippet: TheKey

                      after

                      """;

        List<Snippet>? snippets =
        [
            SnippetBuild("cs", "theKey"),
            SnippetBuild("cs", "TheKey"),
        ];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, snippets, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task WithTwoLineSnippet()
    {
        var content = """

                      before

                      snippet: theKey

                      after

                      """;

        List<Snippet>? snippets =
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
                path: "thePath",
                expressiveCode: null),
        ];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, snippets, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task WithMultiLineSnippet()
    {
        var content = """

                      before

                      snippet: theKey

                      after

                      """;

        List<Snippet>? snippets =
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
                path: "thePath",
                expressiveCode: null)
        ];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, snippets, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task WithDoubleInclude()
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
        IReadOnlyList<Include>? includes =
        [
            Include.Build("theKey", lines, Path.GetFullPath("thePath"))
        ];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, null, null, includes);
        await Verify(output)
            .Snapshot(
                """
                {
                  result:
                before

                theValue1<!-- include: theKey. path: {CurrentDirectory}thePath -->
                theValue2<!-- endInclude -->

                after
                }
                """);
    }

    [Fact]
    public async Task WithEmptyMultipleInclude()
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
        IReadOnlyList<Include>? includes = [Include.Build("theKey", lines, Path.GetFullPath("thePath"))];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, null, null, includes);
        await Verify(output)
            .Snapshot(
                """
                {
                  result:
                before

                <!-- include: theKey. path: {CurrentDirectory}thePath -->

                <!-- endInclude -->

                after
                }
                """);
    }

    [Fact]
    public async Task WithMultipleInclude()
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
        IReadOnlyList<Include>? includes = [Include.Build("theKey", lines, Path.GetFullPath("thePath"))];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, null, null, includes);
        await Verify(output)
            .Snapshot(
                """
                {
                  result:
                before

                theValue1<!-- include: theKey. path: {CurrentDirectory}thePath -->
                theValue2
                theValue3<!-- endInclude -->

                after
                }
                """);
    }

    [Fact]
    public async Task MissingInclude()
    {
        var content = """

                      before

                      include: theKey

                      after

                      """;
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, null, null, null);
        await Verify(output)
            .Snapshot(
                """
                {
                  result:
                before

                ** Could not find include 'theKey' ** <!-- singleLineInclude: theKey -->

                after
                }
                """);
    }

    [Fact]
    public async Task SkipHeadingBeforeToc()
    {
        var content = """

                      ## Heading 1

                      toc

                      Text1

                      ## Heading 2

                      Text2

                      """;
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, null, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task Toc1()
    {
        var content = """

                      # Title

                      toc1

                      ## Heading 1

                      Text1

                      ## Heading 2

                      Text2

                      """;
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, null, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task Toc()
    {
        var content = """

                      # Title

                      toc

                      ## Heading 1

                      Text1

                      ## Heading 2

                      Text2

                      """;
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, null, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task TocRetainedIfNoHeadingsInFile()
    {
        var content = """

                      # Title

                      toc

                      This document has no headings.

                      An empty toc section should be generated, in case
                      any headings are added in future.

                      """;
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, null, null, null);
        await Verify(output);
    }

    [Fact]
    public Task Missing_endToc()
    {
        var content = """

                      <!-- toc -->
                      Bad

                      """;
        var processor = SnippetVerifier.BuildProcessor(DocumentConvention.InPlaceOverwrite, null, null, null);

        return Throws(() => SnippetVerifier.Apply(content, processor))
            .Snapshot(
                """
                {
                  Type: MarkdownProcessingException,
                  File: sourceFile,
                  LineNumber: 2,
                  Message: Expected to find `<!-- endToc -->`. File: sourceFile. LineNumber: 2.
                }
                """);
    }

    [Fact]
    public Task Empty_snippet_key()
    {
        var content = """

                      snippet:


                      """;
        var processor = SnippetVerifier.BuildProcessor(DocumentConvention.InPlaceOverwrite, null, null, null);

        return Throws(() => SnippetVerifier.Apply(content, processor))
            .Snapshot(
                """
                {
                  Type: SnippetException,
                  Message: Could not parse snippet from: snippet:. Path: . Line: 2
                }
                """);
    }

    [Fact]
    public Task Whitespace_snippet_key()
    {
        var content = """

                      snippet:


                      """;
        var processor = SnippetVerifier.BuildProcessor(DocumentConvention.InPlaceOverwrite, null, null, null);

        return Throws(() => SnippetVerifier.Apply(content, processor))
            .Snapshot(
                """
                {
                  Type: SnippetException,
                  Message: Could not parse snippet from: snippet:. Path: . Line: 2
                }
                """);
    }

    [Fact]
    public async Task Toc_Overwrite()
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
        var output = SnippetVerifier.Render(DocumentConvention.InPlaceOverwrite, content, null, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task Simple_Overwrite()
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
        IReadOnlyList<string>? snippetSourceFiles = new List<string>
        {
            Path.Combine(GitRepoDirectoryFinder.FindForFilePath(), "src/Tests/FileToUseAsSnippet.txt")
        };
        var output = SnippetVerifier.Render(DocumentConvention.InPlaceOverwrite, content, availableSnippets, snippetSourceFiles, null);
        await Verify(output);
    }

    [Fact]
    public async Task MixedNewlinesInFile()
    {
        var file = "FileWithMixedNewLines.txt";
        File.Delete(file);
        await File.WriteAllTextAsync(file, "a\rb\nc\r\nd");
        var availableSnippets = new List<Snippet>();
        var content = """

                      some other text

                      snippet: FileWithMixedNewLines.txt

                      """;
        IReadOnlyList<string>? snippetSourceFiles = new List<string>
        {
            file
        };
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, availableSnippets, snippetSourceFiles, null);
        await Verify(output);

        Assert.DoesNotContain("\r\n", output.result);
        Assert.DoesNotContain("\r", output.result);
    }

    [Fact]
    public async Task Simple()
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
        IReadOnlyList<string>? snippetSourceFiles = new List<string>
        {
            Path.Combine(GitRepoDirectoryFinder.FindForFilePath(), "src/Tests/FileToUseAsSnippet.txt")
        };
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, availableSnippets, snippetSourceFiles, null);
        await Verify(output);
    }

    [Fact]
    public async Task SnippetInInclude()
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
        IReadOnlyList<Include>? includes = [Include.Build("theKey", lines, "thePath")];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, availableSnippets, null, includes);
        await Verify(output);
    }

    [Fact]
    public async Task TableInInclude()
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
        IReadOnlyList<Include>? includes = [Include.Build("theKey", lines, "thePath")];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, availableSnippets, null, includes);
        await Verify(output);
    }

    [Fact]
    public async Task SnippetInIncludeLast()
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
        IReadOnlyList<Include>? includes = [Include.Build("theKey", lines, "thePath")];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, availableSnippets, null, includes);
        await Verify(output);
    }

    [Fact]
    public async Task WithIndentedSnippet()
    {
        var content = """

                      before

                          snippet: theKey

                      after

                      """;

        List<Snippet>? snippets = [SnippetBuild("cs", "theKey")];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, snippets, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task WithIndentedSnippetMultipleSpaces()
    {
        var content = """

                      before

                              snippet: theKey

                      after

                      """;

        List<Snippet>? snippets = [SnippetBuild("cs", "theKey")];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, snippets, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task WithIndentedCommentSnippet()
    {
        var content = """

                      before

                          <!-- snippet: theKey -->
                          bad content
                          <!-- endSnippet -->

                      after

                      """;

        List<Snippet>? snippets = [SnippetBuild("cs", "theKey")];
        var output = SnippetVerifier.Render(DocumentConvention.InPlaceOverwrite, content, snippets, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task WithTabIndentedSnippet()
    {
        var content = $"""

                       before

                       {"\t"}snippet: theKey

                       after

                       """;

        List<Snippet>? snippets = [SnippetBuild("cs", "theKey")];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, snippets, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task WithIndentedWebSnippet()
    {
        var content = """

                      before

                          web-snippet: http://example.com/file.cs#snippet1

                      after

                      """;

        List<Snippet>? snippets = [SnippetBuild("cs", "snippet1")];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, snippets, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task WithIndentedMultiLineSnippet()
    {
        var content = """

                      before

                        snippet: theKey

                      after

                      """;

        List<Snippet>? snippets =
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
                path: "thePath",
                expressiveCode: null)
        ];
        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, snippets, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task WithCommentWebSnippetUpdate()
    {
        var content = """

                      before

                      <!-- web-snippet: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt#snipPet -->
                      OLD CONTENT
                      THAT SHOULD BE
                      REPLACED
                      <!-- endSnippet -->

                      after

                      """;

        var output = SnippetVerifier.Render(DocumentConvention.InPlaceOverwrite, content, null, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task WithCommentWebSnippetWithViewUrl()
    {
        var content = """

                      before

                      <!-- web-snippet: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt#snipPet https://github.com/SimonCropp/MarkdownSnippets/blob/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt -->
                      OLD CONTENT
                      THAT SHOULD BE
                      REPLACED
                      <!-- endSnippet -->

                      after

                      """;

        var output = SnippetVerifier.Render(DocumentConvention.InPlaceOverwrite, content, null, null, null);
        await Verify(output);
    }

    [Fact]
    public async Task WithInlineWebSnippetWithViewUrl()
    {
        var content = """

                      before

                      web-snippet: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt#snipPet https://github.com/SimonCropp/MarkdownSnippets/blob/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt

                      after

                      """;

        var output = SnippetVerifier.Render(DocumentConvention.SourceTransform, content, null, null, null);
        await Verify(output);
    }

    static Snippet SnippetBuild(string language, string key) =>
        Snippet.Build(
            language: language,
            startLine: 1,
            endLine: 2,
            value: "Snippet",
            key: key,
            path: "thePath",
            expressiveCode: null);
}