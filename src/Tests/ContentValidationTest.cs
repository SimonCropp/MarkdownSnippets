using System.Threading.Tasks;
using MarkdownSnippets;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ContentValidationTest :
    VerifyBase
{
    [Fact]
    public Task CheckInvalidWord()
    {
        var exception = Assert.Throws<MarkdownProcessingException>(() => ContentValidation.Verify(" you "));
        return Verify(exception.Message);
    }

    [Fact]
    public Task CheckInvalidWordIndicatesAllViolationsInTheExceptionMessage()
    {
        var exception = Assert.Throws<MarkdownProcessingException>(() => ContentValidation.Verify(" you, and you again! Still yourself? "));
        return Verify(exception.Message);
    }

    [Fact]
    public Task CheckInvalidWordIndicatesAllViolationsInTheExceptionMessageIgnoringCase()
    {
        var exception = Assert.Throws<MarkdownProcessingException>(() => ContentValidation.Verify(" you, and you again! Still Yourself? Us"));
        return Verify(exception.Message);
    }

    [Fact]
    public Task CheckInvalidWordWithQuestionMark()
    {
        var exception = Assert.Throws<MarkdownProcessingException>(() => ContentValidation.Verify(" you? "));
        return Verify(exception.Message);
    }

    [Fact]
    public Task CheckInvalidWordWithComma()
    {
        var exception = Assert.Throws<MarkdownProcessingException>(() => ContentValidation.Verify(" you, "));
        return Verify(exception.Message);
    }

    [Fact]
    public Task CheckInvalidWordSentenceEnd()
    {
        var exception = Assert.Throws<MarkdownProcessingException>(() => ContentValidation.Verify(" you. "));
        return Verify(exception.Message);
    }

    [Fact]
    public Task CheckInvalidWordSentenceStart()
    {
        var exception = Assert.Throws<MarkdownProcessingException>(() => ContentValidation.Verify("you "));
        return Verify(exception.Message);
    }

    [Fact]
    public Task CheckInvalidWordStringEnd()
    {
        var exception = Assert.Throws<MarkdownProcessingException>(() => ContentValidation.Verify("the you"));
        return Verify(exception.Message);
    }

    [Fact]
    public void CheckInvalidWordDoesNotThrowWhenNoMatch()
    {
        ContentValidation.Verify(" some random content which doesn't contain invalid words. ");
    }

    [Fact]
    public void CheckInvalidWordDoesNotThrowWhenIsQuote()
    {
        ContentValidation.Verify("> you ");
    }

    [Fact]
    public void CheckInvalidWordInUrl()
    {
       ContentValidation.Verify("some random content containing links /us/allowed/");
       ContentValidation.Verify("some random content containing links /yourself/us/");
       ContentValidation.Verify(" /us/ ");
       ContentValidation.Verify("/us-");
    }

    public ContentValidationTest(ITestOutputHelper output) :
        base(output)
    {
    }
}
