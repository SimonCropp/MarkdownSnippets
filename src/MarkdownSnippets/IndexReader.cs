using System;
using System.IO;

class IndexReader
{
    TextReader textReader;
    public int Index { get; private set; }
    public string NewLine { get; private set; } = Environment.NewLine;

    public IndexReader(TextReader textReader)
    {
        this.textReader = textReader;
    }

    string ReadFirstLine()
    {
        var builder = StringBuilderCache.Acquire();
        try
        {

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
                        NewLine = "\r";
                        break;
                    }

                    if (peek == '\n')
                    {
                        NewLine = "\r\n";
                    }
                    else
                    {
                        NewLine = "\r";
                    }

                    textReader.Read();
                    break;
                }

                if (c == '\n')
                {
                    NewLine = "\n";
                    break;
                }

                builder.Append((char) c);
            } while (true);

            return builder.ToString();
        }
        finally
        {
            StringBuilderCache.Release(builder);
        }
    }

    public string ReadLine()
    {
        Index++;
        if (Index == 1)
        {
            return ReadFirstLine();
        }
        return textReader.ReadLine();
    }

    public bool IsEnd()
    {
        return textReader.Peek() == -1;
    }
}