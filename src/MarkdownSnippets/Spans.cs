ref struct Spans
{
    CharSpan span;
    readonly List<Index> items;

    public Spans(CharSpan span, List<Index> items)
    {
        this.span = span;
        this.items = items;
    }

    public record Index(int From, int To);

    public int Length => items.Count;

    public CharSpan this[int index]
    {
        get
        {
            var tuple = items[index];
            return span.Slice(tuple.From, tuple.To);
        }
    }
}