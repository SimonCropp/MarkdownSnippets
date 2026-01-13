static class Lines
{
    public static void RemoveUntil(
        this List<Line> lines,
        int index,
        string match,
        string? path,
        Line startLine)
    {
        var endIndex = index;
        while (endIndex < lines.Count)
        {
            if (lines[endIndex].Current.Contains(match))
            {
                lines.RemoveRange(index, endIndex - index + 1);
                return;
            }

            endIndex++;
        }

        throw new MarkdownProcessingException($"Expected to find `{match}`.", path, startLine.LineNumber);
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