#nullable disable
public record IngestDestination
{
    public string MachineName { get; set; }
    public string IngestionStrategy { get; set; }
    public IDictionary<string, string> StrategyParams { get; set; }
}