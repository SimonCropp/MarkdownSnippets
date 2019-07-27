using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

static class LineReader
{
    public static (List<Line> lines, string newLine) ReadAllLines(TextReader textReader, string path)
    {
        var lines = new List<Line>();

        var (content, newLine) = ReadFirstLine(textReader);
        lines.Add(new Line(content,path,1));
        var index = 1;
        do
        {
            index++;
            var original = textReader.ReadLine();

            if (original == null)
            {
                break;
            }

            lines.Add(new Line(original, path,index));

        } while (true);

        return (lines, newLine);
    }

    internal static (string content, string newLine) ReadFirstLine(TextReader textReader)
    {
        var builder = new StringBuilder();
        var newLine = Environment.NewLine;
        do
        {
            var c = textReader.Read();
            if (c == -1)
            {
                break;
            }
            if (c == '\r')
            {
                var peek = textReader.Peek();
                if (peek == -1)
                {
                    newLine = "\r";
                    break;
                }
                if (peek == '\n')
                {
                    newLine = "\r\n";
                }
                else
                {
                    newLine = "\r";
                }

                textReader.Read();
                break;
            }

            if (c == '\n')
            {
                newLine = "\n";
                break;
            }

            builder.Append((char)c);
        } while (true);
        return (builder.ToString(),newLine);
    }
}