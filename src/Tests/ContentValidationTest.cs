public class ContentValidationTest
{
    [Fact]
    public Task CheckInvalidWord() => Verify(ContentValidation.Verify(" you "));

    [Fact]
    public Task CheckInvalidWordIndicatesAllViolationsInTheExceptionMessage() =>
        Verify(ContentValidation.Verify(" you, and you again! Still yourself? "));

    [Fact]
    public Task CheckInvalidWordIndicatesAllViolationsInTheExceptionMessageIgnoringCase() =>
        Verify(ContentValidation.Verify(" you, and you again! Still Yourself? Us"));

    [Fact]
    public Task CheckInvalidWordWithQuestionMark() =>
        Verify(ContentValidation.Verify(" you? "));

    [Fact]
    public Task CheckInvalidWordWithComma() =>
        Verify(ContentValidation.Verify(" you, "));

    [Fact]
    public Task CheckInvalidWordSentenceEnd() =>
        Verify(ContentValidation.Verify(" you. "));

    [Fact]
    public Task CheckInvalidWordSentenceStart() =>
        Verify(ContentValidation.Verify("you "));

    [Fact]
    public Task CheckInvalidWordStringEnd() =>
        Verify(ContentValidation.Verify("the you"));

    [Fact]
    public void CheckInvalidWordDoesNotThrowWhenNoMatch() =>
        Assert.Empty(ContentValidation.Verify(" some random content which doesn't contain invalid words. "));

    [Fact]
    public void CheckInvalidWordDoesNotThrowWhenIsQuote() =>
        Assert.Empty(ContentValidation.Verify("> you "));

    [Fact]
    public void CheckInvalidWordInUrl()
    {
        Assert.Empty(ContentValidation.Verify("some random content containing links /us/allowed/"));
        Assert.Empty(ContentValidation.Verify("some random content containing links /yourself/us/"));
        Assert.Empty(ContentValidation.Verify(" /us/ "));
        Assert.Empty(ContentValidation.Verify("/us-"));
    }

    [Fact]
    public void InvalidWordInInlineCodeIsIgnored()
    {
        Assert.Empty(ContentValidation.Verify(" a `you` b "));
        Assert.Empty(ContentValidation.Verify(" see `simple` here "));
    }

    // The null-forgiving operator inside inline code must not trip the exclamation rule.
    [Fact]
    public void ExclamationInInlineCodeIsIgnored() =>
        Assert.Empty(ContentValidation.Verify(" project `_.Department!.Name` out of it "));

    [Fact]
    public Task ValidWordOutsideInlineCodeStillDetected() =>
        Verify(ContentValidation.Verify(" you and `you` "));
}