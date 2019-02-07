using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CaptureSnippets;
using ObjectApproval;
using Xunit;

public class SnippetExtractorTests : TestBase
{
    [Fact]
    public async Task AppendUrlAsSnippet()
    {
        var snippets = new List<Snippet>();
        await snippets.AppendUrlAsSnippet("https://raw.githubusercontent.com/SimonCropp/CaptureSnippets/master/src/appveyor.yml");
        ObjectApprover.VerifyWithJson(snippets);
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
            ObjectApprover.VerifyWithJson(
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
    public void WithDodgyEmDash()
    {
        var input = @"
  <!-- startcode key -->
  —
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void WithDodgyLeftQuote()
    {
        var input = @"
  <!-- startcode key -->
  “
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void WithDodgyRightQuote()
    {
        var input = @"
  <!-- startcode key -->
  ”
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
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
        ObjectApprover.VerifyWithJson(snippets);
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
        ObjectApprover.VerifyWithJson(snippets);
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
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void NestedMixed2()
    {
        var input = @"
  #region KeyParent
  a
  <!-- startcode KeyChild -->
  b
  <!-- endcode -->
  c
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void RemoveDuplicateNewlines()
    {
        var input = @"


  <!-- startcode KeyParent -->


  a


  <!-- startcode KeyChild -->


  b


  <!-- endcode -->


  c


  <!-- endcode -->


";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void NestedStartCode()
    {
        var input = @"
  <!-- startcode KeyParent -->
  a
  <!-- startcode KeyChild -->
  b
  <!-- endcode -->
  c
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void NestedMixed1()
    {
        var input = @"
  <!-- startcode KeyParent -->
  a
  #region KeyChild
  b
  #endregion
  c
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractFromXml()
    {
        var input = @"
  <!-- startcode CodeKey -->
  <configSections/>
  <!-- endcode -->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    public List<Snippet> FromText(string contents)
    {
        using (var stringReader = new StringReader(contents))
        {
            return FileSnippetExtractor.Read(stringReader, "path.cs").ToList();
        }
    }

    [Fact]
    public void UnClosedSnippet()
    {
        var input = @"
  <!-- startcode CodeKey -->
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void UnClosedRegion()
    {
        var input = @"
  #region CodeKey
  <configSections/>";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractFromRegion()
    {
        var input = @"
  #region CodeKey
  The Code
  #endregion";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractWithNoTrailingCharacters()
    {
        var input = @"
  // startcode CodeKey
  the code
  // endcode ";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractWithMissingSpaces()
    {
        var input = @"
  <!--startcode CodeKey-->
  <configSections/>
  <!--endcode-->";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Fact]
    public void CanExtractWithTrailingWhitespace()
    {
        var input = @"
  // startcode CodeKey
  the code
  // endcode   ";
        var snippets = FromText(input);
        ObjectApprover.VerifyWithJson(snippets);
    }
}