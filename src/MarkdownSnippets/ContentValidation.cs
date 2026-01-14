static class ContentValidation
{
    static FrozenDictionary<string, string> phrases = new Dictionary<string, string>
    {
        {"a majority of ", "most"},
        {"a number of", "some or many"},
        {"at an early date", "soon"},
        {"at the conclusion of", "after or following"},
        {"at the present time", "now"},
        {"at this point in time", "now"},
        {"based on the fact that", "because or since"},
        {"despite the fact that", "although"},
        {"due to the fact that", "because"},
        {"during the course of", "during"},
        {"during the time that", "during or while"},
        {"have the capability to", "can"},
        {"in connection with", "about"},
        {"in order to", "to"},
        {"in regard to ", "regarding or about"},
        {"in the event of", "if"},
        {"in view of the fact that", "because"},
        {"it is often the case that", "often"},
        {"make reference to ", "refer to"},
        {"of the opinion that", "think that "},
        {"on a daily basis", "daily"},
        {"on the grounds that", "because"},
        {"prior to", "before"},
        {"so as to", "to"},
        {"subsequent to", "after"},
        {"take into consideration", "consider"},
        {"until such time as", "until"},
        {"a lot", "many"},
        {"sort of", "similar or approximately"},
        {"kind of", "similar or approximately "}
    }.ToFrozenDictionary();

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
