static class SpanSplitter
{
    public static Spans SplitBySpace(this CharSpan value)
    {
        var indexes = new List<Spans.Index>();
        var index = 0;
        int? from = null;
        while (true)
        {
            if (index == value.Length)
            {
                if (from != null)
                {
                    indexes.Add(new (from.Value, index-from.Value));
                }
                break;
            }
            var ch = value[index];

            if (ch == ' ')
            {
                if (from != null)
                {
                    indexes.Add(new (from.Value, index-from.Value));
                }

                from = null;
            }
            else
            {
                if (from == null)
                {
                    from = index;
                }
            }

            index++;
        }

        return new(value,indexes);
    }
}