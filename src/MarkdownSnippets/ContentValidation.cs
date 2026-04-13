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

    static FrozenSet<string> invalidWordSet = new[]
    {
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
    }.ToFrozenSet();

    static FrozenDictionary<string, KeyValuePair<string, string>[]> phrasesByFirstWord =
        phrases
            .GroupBy(p =>
            {
                var spaceIndex = p.Key.IndexOf(' ');
                return spaceIndex == -1 ? p.Key : p.Key[..spaceIndex];
            })
            .ToFrozenDictionary(g => g.Key, g => g.ToArray());

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

        // Tokenize words with positions
        var words = Tokenize(cleanedLine);

        // Check invalid words via set lookup (report first occurrence only)
        var seenWords = new HashSet<string>();
        foreach (var (word, start) in words)
        {
            if (invalidWordSet.Contains(word) && seenWords.Add(word))
            {
                yield return ($"Invalid word detected: '{word}'", start - 1);
            }
        }

        // Check phrases via first-word lookup (report first occurrence only)
        var seenPhrases = new HashSet<string>();
        foreach (var (word, start) in words)
        {
            if (phrasesByFirstWord.TryGetValue(word, out var candidates))
            {
                foreach (var candidate in candidates)
                {
                    if (seenPhrases.Contains(candidate.Key))
                    {
                        continue;
                    }

                    if (cleanedLine.AsSpan(start).StartsWith(candidate.Key.AsSpan(), StringComparison.Ordinal))
                    {
                        seenPhrases.Add(candidate.Key);
                        yield return ($"Invalid phrase detected: '{candidate.Key}'. Instead consider '{candidate.Value}'", start);
                    }
                }
            }
        }
    }

    static List<(string word, int start)> Tokenize(string cleanedLine)
    {
        var words = new List<(string word, int start)>();
        var span = cleanedLine.AsSpan();
        var pos = 0;
        while (pos < span.Length)
        {
            if (span[pos] == ' ')
            {
                pos++;
                continue;
            }

            var wordStart = pos;
            while (pos < span.Length && span[pos] != ' ')
            {
                pos++;
            }

            words.Add((span[wordStart..pos].ToString(), wordStart));
        }

        return words;
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
