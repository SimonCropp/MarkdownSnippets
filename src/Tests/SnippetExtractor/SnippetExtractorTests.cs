using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class SnippetExtractorTests :
    XunitApprovalBase
{
    [Fact]
    public async Task AppendUrlAsSnippet()
    {
        var snippets = new List<Snippet>();
        await snippets.AppendUrlAsSnippet("https://raw.githubusercontent.com/SimonCropp/MarkdownSnippets/master/src/appveyor.yml");
        ObjectApprover.Verify(snippets);
    }

    [Fact]
    public void AppendFileAsSnippet()
    {
        var temp = Path.GetTempFileName();
        try
        {
            File.WriteAllText(temp, "Foo");
            var snippets = new List<Snippet>();
            snippets.AppendFileAsSnippet(temp);
            ObjectApprover.Verify(
                snippets,
                scrubber: x =>
                {
                    var nameWithoutExtension = Path.GetFileNameWithoutExtension(temp);
                    return x
                        .Replace(temp, "FilePath.txt")
                        .Replace(nameWithoutExtension, "File", StringComparison.OrdinalIgnoreCase);
                });
        }
        finally
        {
            File.Delete(temp);
        }
    }

    [Fact]
    public void CanExtractWithInnerWhiteSpace()
    {
        var input = @"
  #region CodeKey

  BeforeWhiteSpace

  AfterWhiteSpace

  #endregion";
        var snippets = FromText(input);
        ObjectApprover.Verify(snippets);
    }

    [Fact]
    public void NestedBroken()
    {
        var input = @"
  #region KeyParent
  a
  #region KeyChild
  b
  c
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.Verify(snippets);
    }

    [Fact]
    public void NestedRegion()
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
        ObjectApprover.Verify(snippets);
    }

    [Fact]
    public void NestedMixed2()
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
        ObjectApprover.Verify(snippets);
    }

    [Fact]
    public void RemoveDuplicateNewlines()
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
        ObjectApprover.Verify(snippets);
    }

    [Fact]
    public void NestedStartCode()
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
        ObjectApprover.Verify(snippets);
    }

    [Fact]
    public void NestedMixed1()
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
        ObjectApprover.Verify(snippets);
    }

    [Fact]
    public void CanExtractFromXml()
    {
        var input = @"
  <!-- begin-snippet: CodeKey -->
  <configSections/>
  <!-- end-snippet -->";
        var snippets = FromText(input);
        ObjectApprover.Verify(snippets);
    }

    public List<Snippet> FromText(string contents)
    {
        using var stringReader = new StringReader(contents);
        return FileSnippetExtractor.Read(stringReader, "path.cs").ToList();
    }

    [Fact]
    public void UnClosedSnippet()
    {
        var input = @"
  <!-- begin-snippet: CodeKey -->
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.Verify(snippets);
    }

    [Fact]
    public void UnClosedRegion()
    {
        var input = @"
  #region CodeKey
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.Verify(snippets);
    }

    [Fact]
    public void CanExtractFromRegion()
    {
        var input = @"
  #region CodeKey
  The Code
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.Verify(snippets);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        var input = @"
  // begin-snippet: CodeKey
  the code
  // end-snippet ";
        var snippets = FromText(input);
        ObjectApprover.Verify(snippets);
    }

    [Fact]
    public void CanExtractWithMissingSpaces()
    {
        var input = @"
  <!--begin-snippet: CodeKey-->
  <configSections/>
  <!--end-snippet-->";
        var snippets = FromText(input);
        ObjectApprover.Verify(snippets);
    }

    [Fact]
    public void CanExtractWithTrailingWhitespace()
    {
        var input = @"
  // begin-snippet: CodeKey
  the code
  // end-snippet   ";
        var snippets = FromText(input);
        ObjectApprover.Verify(snippets);
    }

    public SnippetExtractorTests(ITestOutputHelper output) :
        base(output)
    {
    }
}