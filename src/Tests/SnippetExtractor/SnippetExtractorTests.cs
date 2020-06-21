using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarkdownSnippets;
using VerifyTests;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

[UsesVerify]
public class SnippetExtractorTests
{
    [Fact]
    public async Task AppendUrlAsSnippet()
    {
        var snippets = new List<Snippet>();
        await snippets.AppendUrlAsSnippet("https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/src/appveyor.yml");
        await Verifier.Verify(snippets);
    }

    [Fact]
    public async Task AppendFileAsSnippet()
    {
        var temp = Path.GetTempFileName().ToLowerInvariant();
        try
        {
            File.WriteAllText(temp, "Foo");
            var snippets = new List<Snippet>();
            snippets.AppendFileAsSnippet(temp);
            var settings = new VerifySettings();
            settings.AddScrubber(x =>
            {
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(temp);
                x.Replace(temp, "FilePath.txt");
                x.Replace(nameWithoutExtension, "File");
            });
            await Verifier.Verify(snippets, settings);
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
        return Verifier.Verify(snippets);
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
        return Verifier.Verify(snippets);
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
        return Verifier.Verify(snippets);
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
        return Verifier.Verify(snippets);
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
        return Verifier.Verify(snippets);
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
        return Verifier.Verify(snippets);
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
        return Verifier.Verify(snippets);
    }

    [Fact]
    public Task CanExtractFromXml()
    {
        var input = @"
  <!-- begin-snippet: CodeKey -->
  <configSections/>
  <!-- end-snippet -->";
        var snippets = FromText(input);
        return Verifier.Verify(snippets);
    }

    List<Snippet> FromText(string contents)
    {
        using var stringReader = new StringReader(contents);
        return FileSnippetExtractor.Read(stringReader, "path.cs", 80).ToList();
    }

    [Fact]
    public Task UnClosedSnippet()
    {
        var input = @"
  <!-- begin-snippet: CodeKey -->
  <configSections/>";
        var snippets = FromText(input);
        return Verifier.Verify(snippets);
    }

    [Fact]
    public Task UnClosedRegion()
    {
        var input = @"
  #region CodeKey
  <configSections/>";
        var snippets = FromText(input);
        return Verifier.Verify(snippets);
    }

    [Fact]
    public Task TooWide()
    {
        var input = @"
  #region CodeKey
  caaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaab
  #endregion";
        var snippets = FromText(input);
        return Verifier.Verify(snippets);
    }

    [Fact]
    public Task CanExtractFromRegion()
    {
        var input = @"
  #region CodeKey
  The Code
  #endregion";
        var snippets = FromText(input);
        return Verifier.Verify(snippets);
    }

    [Fact]
    public Task CanExtractWithNoTrailingCharacters()
    {
        var input = @"
  // begin-snippet: CodeKey
  the code
  // end-snippet ";
        var snippets = FromText(input);
        return Verifier.Verify(snippets);
    }

    [Fact]
    public Task CanExtractWithMissingSpaces()
    {
        var input = @"
  <!--begin-snippet: CodeKey-->
  <configSections/>
  <!--end-snippet-->";
        var snippets = FromText(input);
        return Verifier.Verify(snippets);
    }

    [Fact]
    public Task CanExtractWithTrailingWhitespace()
    {
        var input = @"
  // begin-snippet: CodeKey
  the code
  // end-snippet   ";
        var snippets = FromText(input);
        return Verifier.Verify(snippets);
    }
}