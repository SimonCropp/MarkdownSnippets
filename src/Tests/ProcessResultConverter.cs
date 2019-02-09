using System;
using MarkdownSnippets;
using Newtonsoft.Json;

class ProcessResultConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteValue("null");
            return;
        }

        var processResult = (ProcessResult)value;
        writer.WriteStartObject();
        writer.WritePropertyName("missing");
        serializer.Serialize(writer, processResult.MissingSnippets);
        writer.WritePropertyName("usedSnippets");
        serializer.Serialize(writer, processResult.UsedSnippets);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ProcessResult);
    }
}