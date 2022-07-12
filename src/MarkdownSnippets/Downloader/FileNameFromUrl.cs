static class FileNameFromUrl
{
    static List<char> invalid = Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).ToList();

    public static string ConvertToFileName(string url)
    {
        var stringBuilder = new StringBuilder();
        foreach (var ch in url)
        {
            if (invalid.Contains(ch))
            {
                stringBuilder.Append("_");
                continue;
            }

            stringBuilder.Append(ch);
        }

        return stringBuilder.ToString();
    }
}