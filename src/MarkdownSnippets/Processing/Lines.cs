﻿static class Lines
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
                lines.RemoveAt(index);
            if (shouldExit)
            {
                break;
            }
        }
    }

    public static IEnumerable<Line> ReadAllLines(TextReader textReader, string? path)
    {
        var index = 1;
        while (textReader.ReadLine() is { } line)
        {
            yield return new(line, path, index);
            index++;
        }
    }
}