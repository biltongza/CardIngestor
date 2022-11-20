using System.Text.Json.Serialization;
namespace Ngestor.Daemon.Config;

[JsonSerializable(typeof(IngestionConfig))]
internal partial class ConfigJsonContext : JsonSerializerContext
{
}