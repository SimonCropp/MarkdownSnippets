[DebuggerDisplay("Key={Key}")]
class LoopState(string key, Func<string, bool> endFunc, int startLine, int maxWidth, string newLine)
{
    public string GetLines()
    {
        if (builder == null)
        {
            return string.Empty;
        }

        try
        {
            builder.TrimEnd();
            return builder.ToString();
        }
        finally
        {
            StringBuilderCache.Release(builder);
        }
    }

    public void AppendLine(string line)
    {
        builder ??= StringBuilderCache.Acquire();

        if (builder.Length == 0)
        {
            if (line.IsWhiteSpace())
            {
                return;
            }

            CheckWhiteSpace(line, ' ');
            CheckWhiteSpace(line, '\t');
        }
        else
        {
            if (newlineCount < 2)
            {
                builder.Append(newLine);
            }
        }
        var paddingToRemove = line.LastIndexOfSequence(paddingChar, paddingLength);

        var lineLength = line.Length - paddingToRemove;
        if (lineLength > maxWidth)
        {
            throw new LineTooLongException(line.Substring(paddingToRemove,lineLength));
        }
        builder.Append(line, paddingToRemove, lineLength);
        if (line.Length == 0)
        {
            newlineCount++;
        }
        else
        {
            newlineCount = 0;
        }
    }

    void CheckWhiteSpace(string line, char whiteSpace)
    {
        var c = line[0];
        if (c != whiteSpace)
        {
            return;
        }

        paddingChar = whiteSpace;
        for (var index = 1; index < line.Length; index++)
        {
            paddingLength++;
            var ch = line[index];
            if (ch != whiteSpace)
            {
                break;
            }
        }
    }

    StringBuilder? builder;
    public string Key { get; } = key;
    char paddingChar;
    int paddingLength;
    public Func<string, bool> EndFunc { get; } = endFunc;
    public int StartLine { get; } = startLine;
    int newlineCount;
}