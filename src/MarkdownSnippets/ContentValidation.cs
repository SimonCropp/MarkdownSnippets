using System.Collections.Generic;
using System.Linq;

static class ContentValidation
{
    static Dictionary<string, string> phrases = new Dictionary<string, string>
        {
            {"a majority of ", "most"},
            {"a number of", "some, many"},
            {"at an early date", "soon"},
            {"at the conclusion of", "after or following"},
            {"at the present time", "now"},
            {"at this point in time", "now"},
            {"based on the fact that", "because or since"},
            {"despite the fact that", "although"},
            {"due to the fact that", "because"},
            {"during the course of", "during"},
            {"during the time that", "during or while"},
            {"have the capability to ", "can"},
            {"in connection with", "about"},
            {"in order to", "to"},
            {"in regard to ", "regarding or about"},
            {"in the event of", "if"},
            {"in view of the fact that", "because"},
            {"it is often the case that", "often"},
            {"make reference to ", "refer to "},
            {"of the opinion that", "think that "},
            {"on a daily basis", "daily"},
            {"on the grounds that", "because"},
            {"prior to", "before"},
            {"relative to ", "regarding or about"},
            {"so as to", "to"},
            {"subsequent to", "after"},
            {"take into consideration", "consider"},
            {"until such time as", "until"},
        };

    static List<string> invalidStrings;

    static List<string> invalidWords = new List<string>
        {
            "you",
            "we",
            "our",
            "your",
            "us",
            "please",
            "yourself"
        };

    static string invalidWordsJoined;

    static ContentValidation()
    {
        invalidWordsJoined = string.Join(", ", invalidWords);
        invalidStrings = BuildInvalidStrings().ToList();
    }

    static IEnumerable<string> BuildInvalidStrings()
    {
        foreach (var word in invalidWords)
        {
            yield return $" {word} ";
        }
    }

    public static IEnumerable<(string error, int column)> Verify(string line)
    {
        if (line.StartsWith(">"))
        {
            yield break;
        }
        var cleanedLine = Clean(line);

        var message = "No exclamation marks. If a statement is important make it bold. https://www.technicalcommunicationcenter.com/2011/12/30/the-discipline-of-punctuation-in-technical-writing/. ";
        var exclamationIndex1 = cleanedLine.IndexOf("! ");
        if (exclamationIndex1 != -1)
        {
            yield return (message,exclamationIndex1);
        }

        foreach (var invalidString in invalidStrings)
        {
            var indexOf = cleanedLine.IndexOf(invalidString);
            if (indexOf == -1)
            {
                continue;
            }
            yield return ($"Invalid word detected: {invalidString}", indexOf);
        }
    }

    static string Clean(string input)
    {
        var builder = StringBuilderCache.Acquire();
        builder.Append(' ');
        foreach (var ch in input)
        {
            if (ch == '\'' ||
                ch == '?' ||
                ch == '.' ||
                ch == ','
                )
            {
                builder.Append(' ');
            }
            else
            {
                builder.Append(char.ToLowerInvariant(ch));
            }
        }
        builder.Append(' ');
        return builder.ToString();
    }

}