public class SnippetUrlMetadataTests
{
    const string url = "https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt";

    [Test]
    public Task FullFileUrl_LanguageOnlyOverride()
    {
        var output = SnippetVerifier.Render(
            DocumentConvention.SourceTransform,
            $"snippet: {url} (lang=cs)",
            null,
            null,
            null);
        return Verify(output)
            .Snapshot(
                """
                {
                  UsedSnippets: [
                    {
                      Key: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt,
                      Language: cs,
                      Value: Some code,
                      Error: ,
                      FileLocation: null,
                      IsInError: false
                    }
                  ],
                  result:
                <!-- snippet: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt (lang=cs) -->
                ```cs
                Some code
                ```
                <!-- endSnippet -->
                }
                """);
    }

    [Test]
    public Task FullFileUrl_OverridesLanguageAndRendersExpressiveCode()
    {
        var output = SnippetVerifier.Render(
            DocumentConvention.SourceTransform,
            $"snippet: {url} (lang=cs title=code1.txt)",
            null,
            null,
            null);
        return Verify(output)
            .Snapshot(
                """
                {
                  UsedSnippets: [
                    {
                      Key: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt,
                      Language: cs,
                      Value: Some code,
                      Error: ,
                      FileLocation: null,
                      IsInError: false
                    }
                  ],
                  result:
                <!-- snippet: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt (lang=cs title=code1.txt) -->
                ```cs title=code1.txt
                Some code
                ```
                <!-- endSnippet -->
                }
                """);
    }

    [Test]
    public Task FullFileUrl_MetadataWithoutLanguageUsesUrlLanguage()
    {
        var output = SnippetVerifier.Render(
            DocumentConvention.SourceTransform,
            $"snippet: {url} (title=code1.txt)",
            null,
            null,
            null);
        return Verify(output)
            .Snapshot(
                """
                {
                  UsedSnippets: [
                    {
                      Key: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt,
                      Language: txt,
                      Value: Some code,
                      Error: ,
                      FileLocation: null,
                      IsInError: false
                    }
                  ],
                  result:
                <!-- snippet: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt (title=code1.txt) -->
                ```txt title=code1.txt
                Some code
                ```
                <!-- endSnippet -->
                }
                """);
    }

    [Test]
    public async Task FullFileUrl_InvalidLanguageThrows()
    {
        var exception = Assert.Throws<SnippetReadingException>(() =>
            SnippetVerifier.Render(
                DocumentConvention.SourceTransform,
                $"snippet: {url} (lang=CS title=code1.txt)",
                null,
                null,
                null));
        await Assert.That(exception.Message).Contains("lang value must be lowercase alphanumeric.");
        await Assert.That(exception.Message).Contains(url);
    }

    [Test]
    public Task InPlaceFullFileUrl_PreservesMetadataInGeneratedMarker()
    {
        var output = SnippetVerifier.Render(
            DocumentConvention.InPlaceOverwrite,
            $"""
             <!-- snippet: {url} (lang=cs title=code1.txt) -->
             old content
             <!-- endSnippet -->
             """,
            null,
            null,
            null);
        return Verify(output)
            .Snapshot(
                """
                {
                  UsedSnippets: [
                    {
                      Key: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt,
                      Language: cs,
                      Value: Some code,
                      Error: ,
                      FileLocation: null,
                      IsInError: false
                    }
                  ],
                  result:
                <!-- snippet: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt (lang=cs title=code1.txt) -->
                ```cs title=code1.txt
                Some code
                ```
                <!-- endSnippet -->
                }
                """);
    }

    [Test]
    public Task FullFileUrl_MissingKeepsMetadataInMarkerButNotInKey()
    {
        var output = SnippetVerifier.Render(
            DocumentConvention.SourceTransform,
            "snippet: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/doesNotExist.txt (lang=xml title=missing)",
            null,
            null,
            null);
        return Verify(output)
            .Snapshot(
                """
                {
                  MissingSnippets: [
                    {
                      Key: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/doesNotExist.txt,
                      LineNumber: 1
                    }
                  ],
                  result:
                <!-- snippet: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/doesNotExist.txt (lang=xml title=missing) -->
                ```
                ** Could not find snippet 'https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/doesNotExist.txt' **
                ```
                <!-- endSnippet -->
                }
                """);
    }

    [Test]
    public Task NamedSnippetWithUrlKey_AppliesMetadata()
    {
        List<Snippet> snippets =
        [
            Snippet.Build(
                language: "txt",
                startLine: 1,
                endLine: 1,
                value: "the snippet",
                key: "https://example.com/named.txt",
                path: "thePath",
                expressiveCode: null),
        ];
        var output = SnippetVerifier.Render(
            DocumentConvention.SourceTransform,
            "snippet: https://example.com/named.txt (lang=cs title=named)",
            snippets,
            null,
            null);
        return Verify(output)
            .Snapshot(
                """
                {
                  UsedSnippets: [
                    {
                      Key: https://example.com/named.txt,
                      Language: cs,
                      Value: the snippet,
                      Error: ,
                      FileLocation: thePath(1-1),
                      IsInError: false
                    }
                  ],
                  result:
                <!-- snippet: https://example.com/named.txt (lang=cs title=named) -->
                ```cs title=named
                the snippet
                ```
                <!-- endSnippet -->
                }
                """);
    }

    [Test]
    public Task FullFileUrl_EmptyParentheses()
    {
        var output = SnippetVerifier.Render(
            DocumentConvention.SourceTransform,
            $"snippet: {url} ()",
            null,
            null,
            null);
        return Verify(output)
            .Snapshot(
                """
                {
                  UsedSnippets: [
                    {
                      Key: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt,
                      Language: txt,
                      Value: Some code,
                      Error: ,
                      FileLocation: null,
                      IsInError: false
                    }
                  ],
                  result:
                <!-- snippet: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt -->
                ```txt
                Some code
                ```
                <!-- endSnippet -->
                }
                """);
    }

    [Test]
    public async Task FullFileUrl_NoSpaceBeforeParenthesesIsPartOfUrl()
    {
        var input = $"snippet: {url}(lang=xml)";
        await Assert.That(SnippetKey.ExtractSnippet(
            new(input, "path", 1),
            out var key,
            out var language,
            out var expressiveCode)).IsTrue();
        await Assert.That(key).IsEqualTo($"{url}(lang=xml)");
        await Assert.That(language).IsNull();
        await Assert.That(expressiveCode).IsNull();
    }

    [Test]
    public Task FullFileUrl_UppercaseScheme()
    {
        var output = SnippetVerifier.Render(
            DocumentConvention.SourceTransform,
            $"snippet: HTTPS{url[5..]} (lang=cs title=code1.txt)",
            null,
            null,
            null);
        return Verify(output)
            .Snapshot(
                """
                {
                  UsedSnippets: [
                    {
                      Key: HTTPS://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt,
                      Language: cs,
                      Value: Some code,
                      Error: ,
                      FileLocation: null,
                      IsInError: false
                    }
                  ],
                  result:
                <!-- snippet: HTTPS://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt (lang=cs title=code1.txt) -->
                ```cs title=code1.txt
                Some code
                ```
                <!-- endSnippet -->
                }
                """);
    }

    [Test]
    public Task FullFileUrl_ExtraWhitespace()
    {
        var output = SnippetVerifier.Render(
            DocumentConvention.SourceTransform,
            $"snippet: {url} \t (  lang=cs  title=code1.txt\t)",
            null,
            null,
            null);
        return Verify(output)
            .Snapshot(
                """
                {
                  UsedSnippets: [
                    {
                      Key: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt,
                      Language: cs,
                      Value: Some code,
                      Error: ,
                      FileLocation: null,
                      IsInError: false
                    }
                  ],
                  result:
                <!-- snippet: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/DirectorySnippetExtractor/Case/code1.txt (lang=cs title=code1.txt) -->
                ```cs title=code1.txt
                Some code
                ```
                <!-- endSnippet -->
                }
                """);
    }
}
