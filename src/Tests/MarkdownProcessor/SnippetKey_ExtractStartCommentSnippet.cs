﻿public class SnippetKey_ExtractStartCommentSnippet
{
    [Fact]
    public void WithDashes()
    {
        Assert.True(SnippetKey.ExtractStartCommentSnippet(new("<!-- snippet: my-code-snippet -->", "path", 1), out var key));
        Assert.Equal("my-code-snippet", key);
    }

    [Fact]
    public void Simple()
    {
        Assert.True(SnippetKey.ExtractStartCommentSnippet(new("<!-- snippet: snippet -->", "path", 1), out var key));
        Assert.Equal("snippet", key);
    }
}