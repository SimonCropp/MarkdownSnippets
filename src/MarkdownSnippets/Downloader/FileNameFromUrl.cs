static class FileNameFromUrl
{
    public static string ConvertToFileName(string url)
    {
        var invalid = Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).ToList();
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