static class ContentValidation
{
    static FrozenDictionary<string, string> phrases = FrozenDictionary.Create<string, string>([
        new("a majority of ", "most"),
        new("a number of", "some or many"),
        new("at an early date", "soon"),
        new("at the conclusion of", "after or following"),
        new("at the present time", "now"),
        new("at this point in time", "now"),
        new("based on the fact that", "because or since"),
        new("despite the fact that", "although"),
        new("due to the fact that", "because"),
        new("during the course of", "during"),
        new("during the time that", "during or while"),
        new("have the capability to", "can"),
        new("in connection with", "about"),
        new("in order to", "to"),
        new("in regard to ", "regarding or about"),
        new("in the event of", "if"),
        new("in view of the fact that", "because"),
        new("it is often the case that", "often"),
        new("make reference to ", "refer to"),
        new("of the opinion that", "think that "),
        new("on a daily basis", "daily"),
        new("on the grounds that", "because"),
        new("prior to", "before"),
        new("so as to", "to"),
        new("subsequent to", "after"),
        new("take into consideration", "consider"),
        new("until such time as", "until"),
        new("a lot", "many"),
        new("sort of", "similar or approximately"),
        new("kind of", "similar or approximately ")
    ]);

    static List<string> invalidStrings;

    static List<string> invalidWords =
    [
        "you",
        "we",
        "our",
        "your",
        "us",
        "please",
        "yourself",
        "just",
        "simply",
        "simple",
        "easy",
        "feel",
        "think",
        "above-mentioned",
        "aforementioned",
        "foregoing",
        "henceforth",
        "hereafter",
        "heretofore",
        "herewith",
        "thereafter",
        "thereof",
        "therewith",
        "whatsoever",
        "whereat",
        "wherein",
        "whereof"
    ];

    static ContentValidation() =>
        invalidStrings = BuildInvalidStrings().ToList();

    static IEnumerable<string> BuildInvalidStrings() =>
        invalidWords.Select(word => $" {word} ");

    public static IEnumerable<(string error, int column)> Verify(string line)
    {
        if (line.StartsWith('>'))
        {
            yield break;
        }

        var cleanedLine = Clean(line);

        var message = "No exclamation marks. If a statement is important make it bold. https://www.technicalcommunicationcenter.com/2011/12/30/the-discipline-of-punctuation-in-technical-writing/. ";
        var exclamationIndex1 = cleanedLine.IndexOf("! ");
        if (exclamationIndex1 != -1)
        {
            yield return (message, exclamationIndex1);
        }

        foreach (var invalidString in invalidStrings)
        {
            var indexOf = cleanedLine.IndexOf(invalidString);
            if (indexOf == -1)
            {
                continue;
            }

            var error = $"Invalid word detected: '{invalidString.Trim()}'";
            yield return (error, indexOf);
        }

        foreach (var phrase in phrases)
        {
            var indexOf = cleanedLine.IndexOf(phrase.Key);
            if (indexOf == -1)
            {
                continue;
            }

            var error = $"Invalid phrase detected: '{phrase.Key}'. Instead consider '{phrase.Value}'";
            yield return (error, indexOf);
        }
    }

    static string Clean(string input)
    {
        var length = input.Length + 2; // +2 for leading and trailing spaces
        return string.Create(length, input, (span, source) =>
        {
            span[0] = ' ';
            var index = 1;
            foreach (var ch in source)
            {
                if (ch is '\'' or '?' or '.' or ',')
                {
                    span[index++] = ' ';
                }
                else
                {
                    span[index++] = char.ToLowerInvariant(ch);
                }
            }
            span[index] = ' ';
        });
    }
}
