using System.IO;
using System.Text.Json.Serialization;

public static class ConfigReader
{
    public static Config Read(string directory)
    {
        var path = Path.Combine(directory, "msdnippets.json");
        if (!File.Exists(path))
        {
            return null;
        }

        return Parse(File.ReadAllText(path));
    }

    public static Config Parse(string value)
    {
        return JsonSerializer.Parse<Config>(value);
    }
}