using System.IO;

class IndexReader
{
    TextReader textReader;
    public int Index { get; private set; }

    public IndexReader(TextReader textReader)
    {
        this.textReader = textReader;
    }

    public string ReadLine()
    {
        Index++;
        return textReader.ReadLine();
    }

    public bool IsEnd()
    {
        return textReader.Peek() == -1;
    }
}