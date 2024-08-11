class SnippetConverter :
    JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var snippet = (Snippet)value;
        writer.WriteStartObject();
        writer.WritePropertyName("Key");
        serializer.Serialize(writer, snippet.Key);
        if (!snippet.IsInError)
        {
            writer.WritePropertyName("Language");
            serializer.Serialize(writer, snippet.Language);
            writer.WritePropertyName("Value");
            serializer.Serialize(writer, snippet.Value);
        }
        writer.WritePropertyName("Error");
        serializer.Serialize(writer, snippet.Error);
        writer.WritePropertyName("FileLocation");
        serializer.Serialize(writer, snippet.FileLocation);
        writer.WritePropertyName("IsInError");
        serializer.Serialize(writer, snippet.IsInError);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) =>
        throw new NotImplementedException();

    public override bool CanConvert(Type objectType) =>
        objectType == typeof(Snippet);
}