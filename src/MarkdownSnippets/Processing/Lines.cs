using System.Collections.Generic;
using System.IO;
using MarkdownSnippets;

static class Lines
{
    public static void RemoveUntil(
        this List<Line> lines,
        int index,
        string match,
        string? path,
        Line startLine)
    {
        while (true)
        {
            if (index == lines.Count)
            {
                throw new MarkdownProcessingException($"Expected to find `{match}`.", path, startLine.LineNumber);
            }

            var lineCurrent = lines[index].Current;
            var shouldExit = lineCurrent.Contains(match);
            if (shouldExit)
            {
                lines.RemoveAt(index);
                break;
            }

            lines.RemoveAt(index);
        }
    }

    public static IEnumerable<Line> ReadAllLines(TextReader textReader, string? path)
    {
        string? line;
        var index = 1;
        while ((line = textReader.ReadLine()) != null)
        {
            yield return new Line(line, path, index);
            index++;
        }
    }
}