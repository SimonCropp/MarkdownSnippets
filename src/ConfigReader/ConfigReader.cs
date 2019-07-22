using System.IO;
using System.Runtime.Serialization.Json;

public static class ConfigReader
{
    public static Config Read(string directory)
    {
        var path = Path.Combine(directory, "msdnippets.json");
        if (!File.Exists(path))
        {
            return null;
        }

        using (var stream = File.OpenRead(path))
        {
            return Parse(stream);
        }
    }

    public static Config Parse(Stream stream)
    {
        var serializer = new DataContractJsonSerializer(typeof(Config));
        return (Config) serializer.ReadObject(stream);
    }
}