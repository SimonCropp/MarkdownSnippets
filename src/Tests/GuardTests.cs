using System;
using ApprovalTests;
using Xunit;

public class GuardTests : TestBase
{
    [Fact]
    public void DirectoryIsFullyQualified_Invalid()
    {
        var argumentException = Assert.Throws<ArgumentException>(() => Guard.DirectoryIsFullyQualified("foo", "arg"));
        Approvals.Verify(argumentException.Message);
    }

    [Fact]
    public void DirectoryIsFullyQualified_Valid()
    {
        Guard.DirectoryIsFullyQualified("c:/", "arg");
    }
}