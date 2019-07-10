using System;
using System.Diagnostics;
using System.Text;

[DebuggerDisplay("Key={Key}")]
class LoopState
{
    public string GetLines()
    {
        if (builder == null)
        {
            return string.Empty;
        }

        builder.TrimEnd();
        return builder.ToString();
    }

    public void AppendLine(string line)
    {
        if (builder == null)
        {
            builder = new StringBuilder();
        }

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
                builder.AppendLine();
            }
        }
        var paddingToRemove = line.LastIndexOfSequence(paddingChar, paddingLength);

        builder.Append(line, paddingToRemove, line.Length - paddingToRemove);
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

    StringBuilder builder;
    public string Key;
    char paddingChar;
    int paddingLength;

    public Func<string, bool> EndFunc;
    public int StartLine;
    int newlineCount;
}