public class SnippetExtractorTests
{
    [Test]
    public async Task AppendUrlAsSnippet()
    {
        var snippets = new List<Snippet>();
        await snippets.AppendUrlAsSnippet("https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/UrlAsSnippet/sample.yml");
        await Verify(snippets)
            .Snapshot(
                """
                [
                  {
                    Key: sample.yml,
                    Language: yml,
                    Value:
                name: sample
                steps:
                  - run: echo one
                  - run: echo two,
                    Error: ,
                    FileLocation: https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/main/src/Tests/UrlAsSnippet/sample.yml(1-4),
                    IsInError: false
                  }
                ]
                """);
    }

    [Test]
    public async Task AppendUrlAsSnippetInline()
    {
        var snippets = new List<Snippet>();
        await snippets.AppendUrlAsSnippet("https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/src/Tests/Snippets/Usage.cs");
        await Verify(snippets).ScrubLinesContaining("#region", "#endregion");
    }

    [Test]
    public async Task AppendFileAsSnippet()
    {
        var temp = Path.GetTempFileName().ToLowerInvariant();
        try
        {
            await File.WriteAllTextAsync(temp, "Foo");
            var snippets = new List<Snippet>();
            snippets.AppendFileAsSnippet(temp);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(temp);
            await Verify(snippets)
                .ScrubReplace(
                    StringComparison.Ordinal,
                    false,
                    (temp, "FilePath.txt"),
                    (nameWithoutExtension, "File"))
                .Snapshot(
                    """
                    [
                      {
                        Key: File.tmp,
                        Language: tmp,
                        Value: Foo,
                        Error: ,
                        FileLocation: FilePath.txt(1-1),
                        IsInError: false
                      }
                    ]
                    """);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Test]
    public Task CanReadFileWhileLockedByAnotherProcess()
    {
        var temp = Path.Combine(Path.GetTempPath(), "LockedSnippetFile.cs");
        try
        {
            File.WriteAllText(temp,
                """
                #region CodeKey
                The Code
                #endregion
                """);
            using var lockingStream = new FileStream(temp, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            var snippets = FileSnippetExtractor.Read(temp);
            return Verify(snippets)
                .ScrubReplace(temp, "LockedFile.cs")
                .Snapshot(
                    """
                    [
                      {
                        Key: CodeKey,
                        Language: cs,
                        Value: The Code,
                        Error: ,
                        FileLocation: LockedFile.cs(1-3),
                        IsInError: false
                      }
                    ]
                    """);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Test]
    public Task CanExtractWithInnerWhiteSpace()
    {
        var input =
            """

              #region CodeKey

              BeforeWhiteSpace

              AfterWhiteSpace

              #endregion
            """;
        var snippets = FromText(input);
        return Verify(snippets)
            .Snapshot(
                """
                [
                  {
                    Key: CodeKey,
                    Language: cs,
                    Value:
                BeforeWhiteSpace

                AfterWhiteSpace,
                    Error: ,
                    FileLocation: path.cs(2-8),
                    IsInError: false
                  }
                ]
                """);
    }

    [Test]
    public Task NestedBroken()
    {
        var input =
            """

              #region KeyParent
              a
              #region KeyChild
              b
              c
              #endregion
            """;
        var snippets = FromText(input);
        return Verify(snippets)
            .Snapshot(
                """
                [
                  {
                    Key: KeyChild,
                    Language: cs,
                    Value:
                b
                c,
                    Error: ,
                    FileLocation: path.cs(4-7),
                    IsInError: false
                  },
                  {
                    Key: KeyParent,
                    Error: Snippet was not closed,
                    FileLocation: path.cs(3-3),
                    IsInError: true
                  }
                ]
                """);
    }

    [Test]
    public Task NestedRegion()
    {
        var input =
            """

              #region KeyParent
              a
              #region KeyChild
              b
              #endregion
              c
              #endregion
            """;
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Test]
    public Task NestedMixed2()
    {
        var input =
            """
            #region KeyParent
            a
            <!-- begin-snippet: KeyChild -->
            b
            <!-- end-snippet -->
            c
            #endregion
            """;
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Test]
    public Task RemoveDuplicateNewlines()
    {
        var input =
            """

            <!-- begin-snippet: KeyParent -->


            a


            <!-- begin-snippet: KeyChild -->


            b


            <!-- end-snippet -->


            c


            <!-- end-snippet -->



            """;
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Test]
    public Task NestedStartCode()
    {
        var input =
            """
            <!-- begin-snippet: KeyParent -->
            a
            <!-- begin-snippet: KeyChild -->
            b
            <!-- end-snippet -->
            c
            <!-- end-snippet -->
            """;
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Test]
    public Task NestedMixed1()
    {
        var input =
            """
            <!-- begin-snippet: KeyParent -->
            a
            #region KeyChild
            b
            #endregion
            c
            <!-- end-snippet -->
            """;
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Test]
    public Task CanExtractFromXml()
    {
        var input =
            """
            <!-- begin-snippet: CodeKey -->
            <configSections/>
            <!-- end-snippet -->
            """;
        var snippets = FromText(input);
        return Verify(snippets)
            .Snapshot(
                """
                [
                  {
                    Key: CodeKey,
                    Language: cs,
                    Value: <configSections/>,
                    Error: ,
                    FileLocation: path.cs(1-3),
                    IsInError: false
                  }
                ]
                """);
    }

    [Test]
    public Task LanguageOverride()
    {
        var input =
            """
            <!-- begin-snippet: CodeKey (lang=json) -->
            {"a": 1}
            <!-- end-snippet -->
            """;
        var snippets = FromText(input);
        return Verify(snippets)
            .Snapshot(
                """
                [
                  {
                    Key: CodeKey,
                    Language: json,
                    Value: {"a": 1},
                    Error: ,
                    FileLocation: path.cs(1-3),
                    IsInError: false
                  }
                ]
                """);
    }

    [Test]
    public Task LanguageOverrideWithExpressiveCode()
    {
        var input =
            """
            <!-- begin-snippet: CodeKey (lang=json title="config.json") -->
            {"a": 1}
            <!-- end-snippet -->
            """;
        var snippets = FromText(input);
        return Verify(snippets)
            .Snapshot(
                """
                [
                  {
                    Key: CodeKey,
                    Language: json,
                    Value: {"a": 1},
                    Error: ,
                    FileLocation: path.cs(1-3),
                    IsInError: false
                  }
                ]
                """);
    }

    static List<Snippet> FromText(string contents)
    {
        using var reader = new StringReader(contents);
        return FileSnippetExtractor.Read(reader, "path.cs", 80).ToList();
    }

    [Test]
    public Task UnClosedSnippet()
    {
        var input =
            """
            <!-- begin-snippet: CodeKey -->
            <configSections/>
            """;
        var snippets = FromText(input);
        return Verify(snippets)
            .Snapshot(
                """
                [
                  {
                    Key: CodeKey,
                    Error: Snippet was not closed,
                    FileLocation: path.cs(2-2),
                    IsInError: true
                  }
                ]
                """);
    }

    [Test]
    public Task UnClosedRegion()
    {
        var input =
            """

              #region CodeKey
              <configSections/>
            """;
        var snippets = FromText(input);
        return Verify(snippets)
            .Snapshot(
                """
                [
                  {
                    Key: CodeKey,
                    Error: Snippet was not closed,
                    FileLocation: path.cs(3-3),
                    IsInError: true
                  }
                ]
                """);
    }

    [Test]
    public Task TooWide()
    {
        var input =
            """

              #region CodeKey
              caaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaab
              #endregion
            """;
        var snippets = FromText(input);
        return Verify(snippets)
            .Snapshot(
                """
                [
                  {
                    Key: CodeKey,
                    Error: Line too long: caaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaab,
                    FileLocation: path.cs(3-3),
                    IsInError: true
                  }
                ]
                """);
    }

    [Test]
    public async Task MixedNewLines()
    {
        var input = "#region CodeKey\r  A\r\n  B\r  C\n  D\n  #endregion";
        var snippets = FromText(input);
        var single = snippets.Single();
        var value = single.Value;
        await Assert.That(value).DoesNotContain("\r\n");
        await Assert.That(value).DoesNotContain("\r");
        await Verify(single)
            .Snapshot(
                """
                {
                  Key: CodeKey,
                  Language: cs,
                  Value:
                A
                B
                C
                D,
                  Error: ,
                  FileLocation: path.cs(1-6),
                  IsInError: false
                }
                """);
    }

    [Test]
    public Task CanExtractFromRegion()
    {
        var input =
            """

              #region CodeKey
              The Code
              #endregion
            """;
        var snippets = FromText(input);
        return Verify(snippets)
            .Snapshot(
                """
                [
                  {
                    Key: CodeKey,
                    Language: cs,
                    Value: The Code,
                    Error: ,
                    FileLocation: path.cs(2-4),
                    IsInError: false
                  }
                ]
                """);
    }

    [Test]
    public Task CanExtractWithNoTrailingCharacters()
    {
        var input =
            """

              // begin-snippet: CodeKey
              the code
              // end-snippet
            """;
        var snippets = FromText(input);
        return Verify(snippets)
            .Snapshot(
                """
                [
                  {
                    Key: CodeKey,
                    Language: cs,
                    Value: the code,
                    Error: ,
                    FileLocation: path.cs(2-4),
                    IsInError: false
                  }
                ]
                """);
    }

    [Test]
    public Task CanExtractWithMissingSpaces()
    {
        var input =
            """

              <!--begin-snippet: CodeKey-->
              <configSections/>
              <!--end-snippet-->
            """;
        var snippets = FromText(input);
        return Verify(snippets)
            .Snapshot(
                """
                [
                  {
                    Key: CodeKey,
                    Language: cs,
                    Value: <configSections/>,
                    Error: ,
                    FileLocation: path.cs(2-4),
                    IsInError: false
                  }
                ]
                """);
    }

    [Test]
    public Task CanExtractWithTrailingWhitespace()
    {
        var input =
            """

              // begin-snippet: CodeKey
              the code
              // end-snippet
            """;
        var snippets = FromText(input);
        return Verify(snippets)
            .Snapshot(
                """
                [
                  {
                    Key: CodeKey,
                    Language: cs,
                    Value: the code,
                    Error: ,
                    FileLocation: path.cs(2-4),
                    IsInError: false
                  }
                ]
                """);
    }

    [Test]
    public Task CanExtractWithExpressiveCode()
    {
        var input =
            """
              <!--begin-snippet: CodeKey(title="Program.cs" {1-3})-->
              Console.WriteLine("Hello World");
              <!--end-snippet-->
            """;
        var snippets = FromText(input);
        return Verify(snippets)
            .Snapshot(
                """
                [
                  {
                    Key: CodeKey,
                    Language: cs,
                    Value: Console.WriteLine("Hello World");,
                    Error: ,
                    FileLocation: path.cs(1-3),
                    IsInError: false
                  }
                ]
                """);
    }
}