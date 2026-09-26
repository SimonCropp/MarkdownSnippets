public class ContentValidationTest
{
    [Test]
    public Task CheckInvalidWord() => Verify(ContentValidation.Verify(" you "))
        .Snapshot(
            """
            [
              {
                Item1: Invalid word detected: 'you'. The full list of invalid words is: above-mentioned, aforementioned, easy, feel, foregoing, henceforth, hereafter, heretofore, herewith, just, our, please, simple, simply, thereafter, thereof, therewith, think, us, we, whatsoever, whereat, wherein, whereof, you, your, yourself,
                Item2: 1
              }
            ]
            """);

    [Test]
    public Task CheckInvalidWordIndicatesAllViolationsInTheExceptionMessage() =>
        Verify(ContentValidation.Verify(" you, and you again! Still yourself? "))
            .Snapshot(
                """
                [
                  {
                    Item1: No exclamation marks. If a statement is important make it bold. https://www.technicalcommunicationcenter.com/2011/12/30/the-discipline-of-punctuation-in-technical-writing/. ,
                    Item2: 20
                  },
                  {
                    Item1: Invalid word detected: 'you'. The full list of invalid words is: above-mentioned, aforementioned, easy, feel, foregoing, henceforth, hereafter, heretofore, herewith, just, our, please, simple, simply, thereafter, thereof, therewith, think, us, we, whatsoever, whereat, wherein, whereof, you, your, yourself,
                    Item2: 1
                  },
                  {
                    Item1: Invalid word detected: 'yourself'. The full list of invalid words is: above-mentioned, aforementioned, easy, feel, foregoing, henceforth, hereafter, heretofore, herewith, just, our, please, simple, simply, thereafter, thereof, therewith, think, us, we, whatsoever, whereat, wherein, whereof, you, your, yourself,
                    Item2: 27
                  }
                ]
                """);

    [Test]
    public Task CheckInvalidWordIndicatesAllViolationsInTheExceptionMessageIgnoringCase() =>
        Verify(ContentValidation.Verify(" you, and you again! Still Yourself? Us"))
            .Snapshot(
                """
                [
                  {
                    Item1: No exclamation marks. If a statement is important make it bold. https://www.technicalcommunicationcenter.com/2011/12/30/the-discipline-of-punctuation-in-technical-writing/. ,
                    Item2: 20
                  },
                  {
                    Item1: Invalid word detected: 'you'. The full list of invalid words is: above-mentioned, aforementioned, easy, feel, foregoing, henceforth, hereafter, heretofore, herewith, just, our, please, simple, simply, thereafter, thereof, therewith, think, us, we, whatsoever, whereat, wherein, whereof, you, your, yourself,
                    Item2: 1
                  },
                  {
                    Item1: Invalid word detected: 'yourself'. The full list of invalid words is: above-mentioned, aforementioned, easy, feel, foregoing, henceforth, hereafter, heretofore, herewith, just, our, please, simple, simply, thereafter, thereof, therewith, think, us, we, whatsoever, whereat, wherein, whereof, you, your, yourself,
                    Item2: 27
                  },
                  {
                    Item1: Invalid word detected: 'us'. The full list of invalid words is: above-mentioned, aforementioned, easy, feel, foregoing, henceforth, hereafter, heretofore, herewith, just, our, please, simple, simply, thereafter, thereof, therewith, think, us, we, whatsoever, whereat, wherein, whereof, you, your, yourself,
                    Item2: 37
                  }
                ]
                """);

    [Test]
    public Task CheckInvalidWordWithQuestionMark() =>
        Verify(ContentValidation.Verify(" you? "))
            .Snapshot(
                """
                [
                  {
                    Item1: Invalid word detected: 'you'. The full list of invalid words is: above-mentioned, aforementioned, easy, feel, foregoing, henceforth, hereafter, heretofore, herewith, just, our, please, simple, simply, thereafter, thereof, therewith, think, us, we, whatsoever, whereat, wherein, whereof, you, your, yourself,
                    Item2: 1
                  }
                ]
                """);

    [Test]
    public Task CheckInvalidWordWithComma() =>
        Verify(ContentValidation.Verify(" you, "))
            .Snapshot(
                """
                [
                  {
                    Item1: Invalid word detected: 'you'. The full list of invalid words is: above-mentioned, aforementioned, easy, feel, foregoing, henceforth, hereafter, heretofore, herewith, just, our, please, simple, simply, thereafter, thereof, therewith, think, us, we, whatsoever, whereat, wherein, whereof, you, your, yourself,
                    Item2: 1
                  }
                ]
                """);

    [Test]
    public Task CheckInvalidWordSentenceEnd() =>
        Verify(ContentValidation.Verify(" you. "))
            .Snapshot(
                """
                [
                  {
                    Item1: Invalid word detected: 'you'. The full list of invalid words is: above-mentioned, aforementioned, easy, feel, foregoing, henceforth, hereafter, heretofore, herewith, just, our, please, simple, simply, thereafter, thereof, therewith, think, us, we, whatsoever, whereat, wherein, whereof, you, your, yourself,
                    Item2: 1
                  }
                ]
                """);

    [Test]
    public Task CheckInvalidWordSentenceStart() =>
        Verify(ContentValidation.Verify("you "))
            .Snapshot(
                """
                [
                  {
                    Item1: Invalid word detected: 'you'. The full list of invalid words is: above-mentioned, aforementioned, easy, feel, foregoing, henceforth, hereafter, heretofore, herewith, just, our, please, simple, simply, thereafter, thereof, therewith, think, us, we, whatsoever, whereat, wherein, whereof, you, your, yourself
                  }
                ]
                """);

    [Test]
    public Task CheckInvalidWordStringEnd() =>
        Verify(ContentValidation.Verify("the you"))
            .Snapshot(
                """
                [
                  {
                    Item1: Invalid word detected: 'you'. The full list of invalid words is: above-mentioned, aforementioned, easy, feel, foregoing, henceforth, hereafter, heretofore, herewith, just, our, please, simple, simply, thereafter, thereof, therewith, think, us, we, whatsoever, whereat, wherein, whereof, you, your, yourself,
                    Item2: 4
                  }
                ]
                """);

    [Test]
    public async Task CheckInvalidWordDoesNotThrowWhenNoMatch() =>
        await Assert.That(ContentValidation.Verify(" some random content which doesn't contain invalid words. ")).IsEmpty();

    [Test]
    public async Task CheckInvalidWordDoesNotThrowWhenIsQuote() =>
        await Assert.That(ContentValidation.Verify("> you ")).IsEmpty();

    [Test]
    public async Task CheckInvalidWordInUrl()
    {
        await Assert.That(ContentValidation.Verify("some random content containing links /us/allowed/")).IsEmpty();
        await Assert.That(ContentValidation.Verify("some random content containing links /yourself/us/")).IsEmpty();
        await Assert.That(ContentValidation.Verify(" /us/ ")).IsEmpty();
        await Assert.That(ContentValidation.Verify("/us-")).IsEmpty();
    }

    [Test]
    public async Task InvalidWordInInlineCodeIsIgnored()
    {
        await Assert.That(ContentValidation.Verify(" a `you` b ")).IsEmpty();
        await Assert.That(ContentValidation.Verify(" see `simple` here ")).IsEmpty();
    }

    // The null-forgiving operator inside inline code must not trip the exclamation rule.
    [Test]
    public async Task ExclamationInInlineCodeIsIgnored() =>
        await Assert.That(ContentValidation.Verify(" project `_.Department!.Name` out of it ")).IsEmpty();

    [Test]
    public Task ValidWordOutsideInlineCodeStillDetected() =>
        Verify(ContentValidation.Verify(" you and `you` "))
            .Snapshot(
                """
                [
                  {
                    Item1: Invalid word detected: 'you'. The full list of invalid words is: above-mentioned, aforementioned, easy, feel, foregoing, henceforth, hereafter, heretofore, herewith, just, our, please, simple, simply, thereafter, thereof, therewith, think, us, we, whatsoever, whereat, wherein, whereof, you, your, yourself,
                    Item2: 1
                  }
                ]
                """);
}