using System.Text.Json.Serialization;

[JsonSerializable(typeof(IngestionConfig))]
internal partial class ConfigJsonContext : JsonSerializerContext
{
}