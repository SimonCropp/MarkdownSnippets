using MarkdownSnippets;

[UsesVerify]
public class SnippetExtractorTests
{
    [Fact]
    public async Task AppendUrlAsSnippet()
    {
        List<Snippet> snippets = new();
        await snippets.AppendUrlAsSnippet("https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/src/appveyor.yml");
        await Verify(snippets);
    }

    [Fact]
    public async Task AppendUrlAsSnippetInline()
    {
        List<Snippet> snippets = new();
        await snippets.AppendUrlAsSnippet("https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/src/Tests/Snippets/Usage.cs");
        await Verify(snippets).ScrubLinesContaining("#region", "#endregion");
    }

    [Fact]
    public async Task AppendFileAsSnippet()
    {
        var temp = Path.GetTempFileName().ToLowerInvariant();
        try
        {
            await File.WriteAllTextAsync(temp, "Foo");
            List<Snippet> snippets = new();
            snippets.AppendFileAsSnippet(temp);
            VerifySettings settings = new();
            settings.AddScrubber(x =>
            {
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(temp);
                x.Replace(temp, "FilePath.txt");
                x.Replace(nameWithoutExtension, "File");
            });
            await Verify(snippets, settings);
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Fact]
    public Task CanExtractWithInnerWhiteSpace()
    {
        var input = @"
  #region CodeKey

  BeforeWhiteSpace

  AfterWhiteSpace

  #endregion";
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Fact]
    public Task NestedBroken()
    {
        var input = @"
  #region KeyParent
  a
  #region KeyChild
  b
  c
  #endregion";
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Fact]
    public Task NestedRegion()
    {
        var input = @"
  #region KeyParent
  a
  #region KeyChild
  b
  #endregion
  c
  #endregion";
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Fact]
    public Task NestedMixed2()
    {
        var input = @"
  #region KeyParent
  a
  <!-- begin-snippet: KeyChild -->
  b
  <!-- end-snippet -->
  c
  #endregion";
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Fact]
    public Task RemoveDuplicateNewlines()
    {
        var input = @"


  <!-- begin-snippet: KeyParent -->


  a


  <!-- begin-snippet: KeyChild -->


  b


  <!-- end-snippet -->


  c


  <!-- end-snippet -->


";
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Fact]
    public Task NestedStartCode()
    {
        var input = @"
  <!-- begin-snippet: KeyParent -->
  a
  <!-- begin-snippet: KeyChild -->
  b
  <!-- end-snippet -->
  c
  <!-- end-snippet -->";
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Fact]
    public Task NestedMixed1()
    {
        var input = @"
  <!-- begin-snippet: KeyParent -->
  a
  #region KeyChild
  b
  #endregion
  c
  <!-- end-snippet -->";
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Fact]
    public Task CanExtractFromXml()
    {
        var input = @"
  <!-- begin-snippet: CodeKey -->
  <configSections/>
  <!-- end-snippet -->";
        var snippets = FromText(input);
        return Verify(snippets);
    }

    static List<Snippet> FromText(string contents)
    {
        using StringReader reader = new(contents);
        return FileSnippetExtractor.Read(reader, "path.cs", 80).ToList();
    }

    [Fact]
    public Task UnClosedSnippet()
    {
        var input = @"
  <!-- begin-snippet: CodeKey -->
  <configSections/>";
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Fact]
    public Task UnClosedRegion()
    {
        var input = @"
  #region CodeKey
  <configSections/>";
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Fact]
    public Task TooWide()
    {
        var input = @"
  #region CodeKey
  caaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaab
  #endregion";
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Fact]
    public Task MixedNewLines()
    {
        var input = "#region CodeKey\r  A\r\n  B\r  C\n  D\n  #endregion";
        var snippets = FromText(input);
        var single = snippets.Single();
        var value = single.Value;
        Assert.DoesNotContain("\r\n", value);
        Assert.DoesNotContain("\r", value);
        return Verify(single);
    }

    [Fact]
    public Task CanExtractFromRegion()
    {
        var input = @"
  #region CodeKey
  The Code
  #endregion";
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Fact]
    public Task CanExtractWithNoTrailingCharacters()
    {
        var input = @"
  // begin-snippet: CodeKey
  the code
  // end-snippet ";
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Fact]
    public Task CanExtractWithMissingSpaces()
    {
        var input = @"
  <!--begin-snippet: CodeKey-->
  <configSections/>
  <!--end-snippet-->";
        var snippets = FromText(input);
        return Verify(snippets);
    }

    [Fact]
    public Task CanExtractWithTrailingWhitespace()
    {
        var input = @"
  // begin-snippet: CodeKey
  the code
  // end-snippet   ";
        var snippets = FromText(input);
        return Verify(snippets);
    }
}