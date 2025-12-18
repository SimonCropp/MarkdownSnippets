public class NewLineConfigReaderTests
{
    [Fact]
    public void GitAttributes_WildcardEolLf()
    {
        var directory = new TempDirectory();

        File.WriteAllText(Path.Combine(directory, ".gitattributes"), "* text eol=lf");
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal("\n", result);
    }

    [Fact]
    public void GitAttributes_WildcardEolCrlf()
    {
        var directory = new TempDirectory();
        File.WriteAllText(Path.Combine(directory, ".gitattributes"), "* text eol=crlf");
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal("\r\n", result);
    }

    [Fact]
    public void GitAttributes_MdSpecificEolLf()
    {
        var directory = new TempDirectory();
        File.WriteAllText(Path.Combine(directory, ".gitattributes"), "*.md text eol=lf");
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal("\n", result);
    }

    [Fact]
    public void GitAttributes_MdSpecificOverridesWildcard()
    {
        var directory = new TempDirectory();
        File.WriteAllText(
            Path.Combine(directory, ".gitattributes"),
            """
            * text eol=crlf
            *.md text eol=lf
            """);
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal("\n", result);
    }

    [Fact]
    public void GitAttributes_NoEolSetting_FallsBackToEnvironmentNewLine()
    {
        var directory = new TempDirectory();
        File.WriteAllText(Path.Combine(directory, ".gitattributes"), "* text");
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal(Environment.NewLine, result);
    }

    [Fact]
    public void GitAttributes_IgnoresComments()
    {
        var directory = new TempDirectory();
        File.WriteAllText(
            Path.Combine(directory, ".gitattributes"),
            """
            # comment eol=crlf
            * text eol=lf
            """);
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal("\n", result);
    }

    [Fact]
    public void GitAttributes_InParentDirectory()
    {
        var directory = new TempDirectory();
        var childDir = Path.Combine(directory, "child");
        Directory.CreateDirectory(childDir);
        File.WriteAllText(Path.Combine(directory, ".gitattributes"), "* text eol=lf");
        var result = NewLineConfigReader.ReadNewLine(childDir, []);
        Assert.Equal("\n", result);
    }

    [Fact]
    public void EditorConfig_WildcardEndOfLineLf()
    {
        var directory = new TempDirectory();
        File.WriteAllText(
            Path.Combine(directory, ".editorconfig"),
            """
            [*]
            end_of_line = lf
            """);
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal("\n", result);
    }

    [Fact]
    public void EditorConfig_WildcardEndOfLineCrlf()
    {
        var directory = new TempDirectory();
        File.WriteAllText(
            Path.Combine(directory, ".editorconfig"),
            """
            [*]
            end_of_line = crlf
            """);
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal("\r\n", result);
    }

    [Fact]
    public void EditorConfig_MdSpecificEndOfLine()
    {
        var directory = new TempDirectory();
        File.WriteAllText(
            Path.Combine(directory, ".editorconfig"),
            """
            [*.md]
            end_of_line = lf
            """);
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal("\n", result);
    }

    [Fact]
    public void EditorConfig_MdSpecificOverridesWildcard()
    {
        var directory = new TempDirectory();
        File.WriteAllText(
            Path.Combine(directory, ".editorconfig"),
            """
            [*]
            end_of_line = crlf

            [*.md]
            end_of_line = lf
            """);
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal("\n", result);
    }

    [Fact]
    public void EditorConfig_BracePattern()
    {
        var directory = new TempDirectory();
        File.WriteAllText(
            Path.Combine(directory, ".editorconfig"),
            """
            [*.{md,txt}]
            end_of_line = lf
            """);
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal("\n", result);
    }

    [Fact]
    public void EditorConfig_IgnoresComments()
    {
        var directory = new TempDirectory();
        File.WriteAllText(
            Path.Combine(directory, ".editorconfig"),
            """
            # comment
            ; another comment
            [*]
            end_of_line = lf
            """);
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal("\n", result);
    }

    [Fact]
    public void GitAttributes_TakesPriorityOverEditorConfig()
    {
        var directory = new TempDirectory();
        File.WriteAllText(Path.Combine(directory, ".gitattributes"), "* text eol=crlf");
        File.WriteAllText(
            Path.Combine(directory, ".editorconfig"),
            """
            [*]
            end_of_line = lf
            """);
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal("\r\n", result);
    }

    [Fact]
    public void NoConfigFiles_FallsBackToEnvironmentNewLine()
    {
        var directory = new TempDirectory();
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal(Environment.NewLine, result);
    }

    [Fact]
    public void EditorConfig_FallbackWhenGitAttributesHasNoEol()
    {
        var directory = new TempDirectory();
        File.WriteAllText(Path.Combine(directory, ".gitattributes"), "* text");
        File.WriteAllText(
            Path.Combine(directory, ".editorconfig"),
            """
            [*]
            end_of_line = lf
            """);
        var result = NewLineConfigReader.ReadNewLine(directory, []);
        Assert.Equal("\n", result);
    }

    [Fact]
    public void DetectsNewLineFromMdFiles_Lf()
    {
        var directory = new TempDirectory();
        var mdFile = Path.Combine(directory, "test.md");
        File.WriteAllText(mdFile, "line1\nline2\n");
        var result = NewLineConfigReader.ReadNewLine(directory, [mdFile]);
        Assert.Equal("\n", result);
    }

    [Fact]
    public void DetectsNewLineFromMdFiles_Crlf()
    {
        var directory = new TempDirectory();
        var mdFile = Path.Combine(directory, "test.md");
        File.WriteAllText(mdFile, "line1\r\nline2\r\n");
        var result = NewLineConfigReader.ReadNewLine(directory, [mdFile]);
        Assert.Equal("\r\n", result);
    }

    [Fact]
    public void ConfigTakesPriorityOverMdFileDetection()
    {
        var directory = new TempDirectory();
        File.WriteAllText(Path.Combine(directory, ".gitattributes"), "* text eol=lf");
        var mdFile = Path.Combine(directory, "test.md");
        File.WriteAllText(mdFile, "line1\r\nline2\r\n");
        var result = NewLineConfigReader.ReadNewLine(directory, [mdFile]);
        Assert.Equal("\n", result);
    }

    [Fact]
    public void MdFileDetection_PicksShortestFileFirst()
    {
        var directory = new TempDirectory();
        var shortFile = Path.Combine(directory, "a.md");
        var longFile = Path.Combine(directory, "longer-name.md");
        File.WriteAllText(shortFile, "line1\nline2\n");
        File.WriteAllText(longFile, "line1\r\nline2\r\n");
        var result = NewLineConfigReader.ReadNewLine(directory, [longFile, shortFile]);
        Assert.Equal("\n", result);
    }

    [Fact]
    public void MdFileDetection_SkipsFilesWithNoNewlines()
    {
        var directory = new TempDirectory();
        var noNewlineFile = Path.Combine(directory, "a.md");
        var withNewlineFile = Path.Combine(directory, "bb.md");
        File.WriteAllText(noNewlineFile, "no newlines here");
        File.WriteAllText(withNewlineFile, "has\nnewlines");
        var result = NewLineConfigReader.ReadNewLine(directory, [noNewlineFile, withNewlineFile]);
        Assert.Equal("\n", result);
    }
}