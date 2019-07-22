using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

public static class ConfigReader
{
    public static Config Read(string directory)
    {
        var path = Path.Combine(directory, "mdsnippets.json");
        if (!File.Exists(path))
        {
            return null;
        }

        return Parse(File.ReadAllText(path));
    }

    public static Config Parse(string contents)
    {
        using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
        {
            memoryStream.Position = 0;
            var serializer = new DataContractJsonSerializer(typeof(Config));
            return (Config) serializer.ReadObject(memoryStream);
        }
    }
}