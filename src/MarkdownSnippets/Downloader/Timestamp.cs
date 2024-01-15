class Timestamp
{
    static DateTime minFileDate = DateTime.FromFileTimeUtc(0);
    public DateTime? Expiry;
    public DateTime? LastModified;

    public static Timestamp GetTimestamp(HttpResponseMessage headResponse)
    {
        var timestamp = new Timestamp();
        var headers = headResponse.Content.Headers;
        if (headers.LastModified != null)
        {
            timestamp.LastModified = headers.LastModified.Value.UtcDateTime;
        }

        if (headers.Expires != null)
        {
            timestamp.Expiry = headers.Expires.Value.UtcDateTime;
        }

        return timestamp;
    }

    public  static void SetTimestamp(string path, Timestamp timestamp)
    {
        File.SetCreationTimeUtc(path, timestamp.LastModified.GetValueOrDefault(DateTime.UtcNow));
        File.SetLastWriteTimeUtc(path, timestamp.Expiry.GetValueOrDefault(minFileDate));
    }

    public static Timestamp GetTimestamp(string path)
    {
        var timestamp = new Timestamp();
        var creationTimeUtc = File.GetCreationTimeUtc(path);
        if (creationTimeUtc != minFileDate)
        {
            timestamp.LastModified = creationTimeUtc;
        }

        var lastWriteTimeUtc = File.GetLastWriteTimeUtc(path);
        if (lastWriteTimeUtc != minFileDate)
        {
            timestamp.Expiry = lastWriteTimeUtc;
        }

        return timestamp;
    }
}