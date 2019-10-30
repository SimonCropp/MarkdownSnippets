using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

public class IncludeFileReaderTests :
    XunitApprovalBase
{
    [Fact]
    public void Simple()
    {
        var files = IncludeFileReader.ReadIncludes(
            new List<string>
            {
                @"IncludeFileReader\file.include.md"
            });
        ObjectApprover.Verify(files("file"));
    }

    [Fact]
    public void Missing()
    {
        var files = IncludeFileReader.ReadIncludes(new List<string>());
        var exception = Assert.Throws<Exception>(() => files("missing"));
        ObjectApprover.Verify(exception.Message);
    }

    public IncludeFileReaderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}