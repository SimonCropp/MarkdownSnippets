using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ContentValidationTest :
    VerifyBase
{
    [Fact]
    public Task CheckInvalidWord()
    {
        return Verify(ContentValidation.Verify(" you "));
    }

    [Fact]
    public Task CheckInvalidWordIndicatesAllViolationsInTheExceptionMessage()
    {
        return Verify(ContentValidation.Verify(" you, and you again! Still yourself? "));
    }

    [Fact]
    public Task CheckInvalidWordIndicatesAllViolationsInTheExceptionMessageIgnoringCase()
    {
        return Verify(ContentValidation.Verify(" you, and you again! Still Yourself? Us"));
    }

    [Fact]
    public Task CheckInvalidWordWithQuestionMark()
    {
        return Verify(ContentValidation.Verify(" you? "));
    }

    [Fact]
    public Task CheckInvalidWordWithComma()
    {
        return Verify(ContentValidation.Verify(" you, "));
    }

    [Fact]
    public Task CheckInvalidWordSentenceEnd()
    {
        return Verify(ContentValidation.Verify(" you. "));
    }

    [Fact]
    public Task CheckInvalidWordSentenceStart()
    {
        return Verify(ContentValidation.Verify("you "));
    }

    [Fact]
    public Task CheckInvalidWordStringEnd()
    {
        return Verify(ContentValidation.Verify("the you"));
    }

    [Fact]
    public void CheckInvalidWordDoesNotThrowWhenNoMatch()
    {
        Assert.Empty(ContentValidation.Verify(" some random content which doesn't contain invalid words. "));
    }

    [Fact]
    public void CheckInvalidWordDoesNotThrowWhenIsQuote()
    {
        Assert.Empty(ContentValidation.Verify("> you "));
    }

    [Fact]
    public void CheckInvalidWordInUrl()
    {
        Assert.Empty(ContentValidation.Verify("some random content containing links /us/allowed/"));
        Assert.Empty(ContentValidation.Verify("some random content containing links /yourself/us/"));
        Assert.Empty(ContentValidation.Verify(" /us/ "));
        Assert.Empty(ContentValidation.Verify("/us-"));
    }

    public ContentValidationTest(ITestOutputHelper output) :
        base(output)
    {
    }
}